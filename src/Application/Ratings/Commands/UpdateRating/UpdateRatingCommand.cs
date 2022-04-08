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

namespace HikingTrailsApi.Application.Ratings.Commands.UpdateRating
{
    public class UpdateRatingCommand : IRequest<Result<RatingVm>>
    {
        public Guid Id { get; set; }
        public bool IsPositive { get; set; }
    }

    public class UpdateRatingCommandHandler : IRequestHandler<UpdateRatingCommand, Result<RatingVm>>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateRatingCommandHandler(IApplicationDbContext applicationDbContext,
            IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<RatingVm>> Handle(UpdateRatingCommand request, CancellationToken cancellationToken)
        {
            var rating = await _applicationDbContext.Ratings
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (rating == null)
            {
                return Result<RatingVm>.NotFound("Id", "Nepavyko surasti įvertinimo");  //404
            }

            Guid.TryParse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("id"), out var userId);

            if (userId != rating.UserId)
            {
                return Result<RatingVm>.Forbidden("Authorization", "Naudotojas negali keisti kitų naudotojų įvertinimų");   //403
            }

            if (rating.IsPositive == request.IsPositive)
            {
                return Result<RatingVm>.Success(_mapper.Map<Rating, RatingVm>(rating)); //200
            }

            var ratingChangeFactor = (request.IsPositive ? 1 : -1) * 2;
            if (rating.PostId.HasValue)
            {
                await _applicationDbContext.Posts
                    .Where(x => x.Id == rating.PostId.Value && !x.IsDeleted)
                    .UpdateAsync(x => new Post { Rating = x.Rating + ratingChangeFactor }, cancellationToken);
            }

            if (rating.CommentId.HasValue)
            {
                await _applicationDbContext.Comments
                    .Where(x => x.Id == rating.CommentId.Value && !x.IsDeleted)
                    .UpdateAsync(x => new Comment { Rating = x.Rating + ratingChangeFactor }, cancellationToken);
            }

            await _applicationDbContext.Ratings
                    .Where(x => x.Id == rating.Id)
                    .UpdateAsync(x => new Rating { IsPositive = request.IsPositive }, cancellationToken);

            rating.IsPositive = request.IsPositive;

            //TODO: Might need to fetch Rating creator(User) to properly map to RatingVm
            return Result<RatingVm>.Success(_mapper.Map<Rating, RatingVm>(rating)); //200
        }
    }
}
