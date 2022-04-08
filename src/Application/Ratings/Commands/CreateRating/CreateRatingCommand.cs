using AutoMapper;
using HikingTrailsApi.Application.Common.Interfaces;
using HikingTrailsApi.Application.Common.Models;
using HikingTrailsApi.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace HikingTrailsApi.Application.Ratings.Commands.CreateRating
{
    public class CreateRatingCommand : IRequest<Result<RatingVm>>
    {
        public bool IsPositive { get; set; }
        public Guid? PostId { get; set; }
        public Guid? CommentId { get; set; }
    }

    public class CreateRatingCommandHandler : IRequestHandler<CreateRatingCommand, Result<RatingVm>>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDateTime _dateTime;

        public CreateRatingCommandHandler(IApplicationDbContext applicationDbContext,
            IMapper mapper, IHttpContextAccessor httpContextAccessor, IDateTime dateTime)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _dateTime = dateTime;
        }

        public async Task<Result<RatingVm>> Handle(CreateRatingCommand request, CancellationToken cancellationToken)
        {
            var createRatingCommandValidator = new CreateRatingCommandValidator();
            var validationResult = await createRatingCommandValidator
                .ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Result<RatingVm>.BadRequest(validationResult.Errors.Select(x =>
                    new FieldError(x.PropertyName, x.ErrorMessage)));   //400
            }

            Guid.TryParse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("id"), out var userId);

            var ratingChangeFactor = request.IsPositive ? 1 : -1;

            if (request.PostId.HasValue)
            {
                var alreadyCreatedRating = await _applicationDbContext.Ratings
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.UserId == userId && x.PostId == request.PostId.Value, cancellationToken);

                if (alreadyCreatedRating != null)
                {
                    return Result<RatingVm>.Conflict("PostId", "Negalima įrašo vertinti dar kartą");    //409
                }

                var post = await _applicationDbContext.Posts
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == request.PostId.Value, cancellationToken);

                if (post == null)
                {
                    return Result<RatingVm>.NotFound("PostId", "Nurodytas įrašas neegzistuoja");    //404
                }

                await _applicationDbContext.Posts
                    .Where(x => x.Id == request.PostId.Value && !x.IsDeleted)
                    .UpdateAsync(x => new Post { Rating = x.Rating + ratingChangeFactor }, cancellationToken);
            }

            if (request.CommentId.HasValue)
            {
                var alreadyCreatedRating = await _applicationDbContext.Ratings
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.UserId == userId && x.CommentId == request.CommentId.Value, cancellationToken);

                if (alreadyCreatedRating != null)
                {
                    return Result<RatingVm>.Conflict("CommentId", "Negalima komentaro vertinti dar kartą"); //409
                }

                var comment = await _applicationDbContext.Comments
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == request.CommentId.Value, cancellationToken);

                if (comment == null)
                {
                    return Result<RatingVm>.NotFound("CommentId", "Nurodytas komentaras neegzistuoja"); //404
                }

                await _applicationDbContext.Comments
                    .Where(x => x.Id == request.CommentId.Value && !x.IsDeleted)
                    .UpdateAsync(x => new Comment { Rating = x.Rating + ratingChangeFactor }, cancellationToken);
            }

            var rating = new Rating
            {
                IsPositive = request.IsPositive,
                CreationDate = _dateTime.Now,
                UserId = userId,
                PostId = request.PostId,
                CommentId = request.CommentId
            };

            _applicationDbContext.Ratings.Add(rating);
            await _applicationDbContext.SaveChangesAsync(cancellationToken);

            //TODO: Might need changing. Mapping from Rating to RatingVm without User entity the RatingVm user fullName can't be formed

            return Result<RatingVm>.Created(_mapper.Map<Rating, RatingVm>(rating)); //201
        }
    }
}
