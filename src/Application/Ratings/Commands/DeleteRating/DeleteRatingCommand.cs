using HikingTrailsApi.Application.Common.Interfaces;
using HikingTrailsApi.Application.Common.Models;
using HikingTrailsApi.Domain.Entities;
using HikingTrailsApi.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace HikingTrailsApi.Application.Ratings.Commands.DeleteRating
{
    public class DeleteRatingCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }

    public class DeleteRatingCommandHandler : IRequestHandler<DeleteRatingCommand, Result>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeleteRatingCommandHandler(IApplicationDbContext applicationDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _applicationDbContext = applicationDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result> Handle(DeleteRatingCommand request, CancellationToken cancellationToken)
        {
            var rating = await _applicationDbContext.Ratings
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (rating == null)
            {
                return Result.NotFound("Id", "Nepavyko surasti įvertinimo");    //404
            }

            var userRole = _httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);
            Guid.TryParse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("id"), out var userId);

            if (userId != rating.UserId && userRole != Role.Administrator.ToString())
            {
                return Result.Forbidden("Authorization", "Naudotojas negali trinti kitų naudotojų įvertinimų");  //403
            }

            var ratingChangeFactor = rating.IsPositive ? 1 : -1;

            if (rating.PostId.HasValue)
            {
                await _applicationDbContext.Posts
                    .Where(x => x.Id == rating.PostId.Value && !x.IsDeleted)
                    .UpdateAsync(x => new Post { Rating = x.Rating - ratingChangeFactor }, cancellationToken);
            }

            if (rating.CommentId.HasValue)
            {
                await _applicationDbContext.Comments
                    .Where(x => x.Id == rating.CommentId.Value && !x.IsDeleted)
                    .UpdateAsync(x => new Comment { Rating = x.Rating - ratingChangeFactor }, cancellationToken);
            }

            _applicationDbContext.Ratings.Remove(rating);
            await _applicationDbContext.SaveChangesAsync(cancellationToken);

            return Result.NoContent();  //204
        }
    }
}
