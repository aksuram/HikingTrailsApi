using AutoMapper;
using AutoMapper.QueryableExtensions;
using HikingTrailsApi.Application.Common.Interfaces;
using HikingTrailsApi.Application.Common.Models;
using HikingTrailsApi.Application.Ratings;
using HikingTrailsApi.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HikingTrailsApi.Application.Posts.Queries.GetPost
{
    public class GetPostQuery : IRequest<Result<PostWithUserRatingVm>>
    {
        public Guid Id { get; set; }
    }

    public class GetPostQueryHandler : IRequestHandler<GetPostQuery, Result<PostWithUserRatingVm>>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetPostQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<PostWithUserRatingVm>> Handle(GetPostQuery request, CancellationToken cancellationToken)
        {
            var post = await _applicationDbContext.Posts
                .AsNoTracking()
                .Include(x => x.User)
                .ThenInclude(x => x.Images)
                .ProjectTo<PostWithUserRatingVm>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (post == null)
            {
                return Result<PostWithUserRatingVm>.NotFound("Id", "Nepavyko surasti įrašo"); //404
            }

            if (_httpContextAccessor?.HttpContext?.User?.Identity.IsAuthenticated ?? false)
            {
                var userId = new Guid(_httpContextAccessor.HttpContext.User.Claims
                    .FirstOrDefault(x => x.Type == "id").Value);

                var rating = await _applicationDbContext.Ratings
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.User.Id == userId && x.PostId == post.Id,
                        cancellationToken);

                if (rating != null) post.UserRating = _mapper.Map<Rating, RatingVm>(rating);
            }

            //TODO: Add user rating to viewmodel?

            return Result<PostWithUserRatingVm>.Success(post); //200
        }
    }
}
