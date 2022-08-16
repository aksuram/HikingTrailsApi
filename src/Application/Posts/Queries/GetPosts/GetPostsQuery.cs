using AutoMapper;
using AutoMapper.QueryableExtensions;
using HikingTrailsApi.Application.Common.Interfaces;
using HikingTrailsApi.Application.Common.Mappings;
using HikingTrailsApi.Application.Common.Models;
using HikingTrailsApi.Application.Ratings;
using HikingTrailsApi.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace HikingTrailsApi.Application.Posts.Queries.GetPosts
{
    public class GetPostsQuery : IRequest<Result<PaginatedList<PostWithUserRatingVm>>>, IPaginatedListQuery
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetPostsQueryHandler : IRequestHandler<GetPostsQuery, Result<PaginatedList<PostWithUserRatingVm>>>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetPostsQueryHandler(IApplicationDbContext applicationDbContext,
            IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<PaginatedList<PostWithUserRatingVm>>> Handle(GetPostsQuery request, CancellationToken cancellationToken)
        {
            var paginatedListValidator = new PaginatedListValidator();
            var validationResult = await paginatedListValidator
                .ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Result<PaginatedList<PostWithUserRatingVm>>.BadRequest(validationResult.Errors.Select(x =>
                    new FieldError(x.PropertyName, x.ErrorMessage)));   //400
            }

            var posts = await _applicationDbContext.Posts
                .AsNoTracking()
                .Include(x => x.User)
                .ThenInclude(x => x.Images)
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreationDate)
                .ProjectTo<PostWithUserRatingVm>(_mapper.ConfigurationProvider)
                .PaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);

            if (posts.Items.Count == 0)
            {
                return Result<PaginatedList<PostWithUserRatingVm>>.NotFound("PageNumber", "Nepavyko surasti įrašų duotame puslapyje");    //404
            }

            if (_httpContextAccessor?.HttpContext?.User?.Identity.IsAuthenticated ?? false)
            {
                var userId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "id");
                var whereExpression = BuildUserRatingsForPostsWhereExpression(new Guid(userId.Value), posts);
                var ratings = await _applicationDbContext.Ratings
                    .AsNoTracking()
                    .Where(whereExpression)
                    .ToListAsync();

                posts = AddUserRatingsToPosts(ratings, posts);
            }

            return Result<PaginatedList<PostWithUserRatingVm>>.Success(posts);   //200
        }

        //TODO: Could probably figure out a proper sql query rather than this
        public Expression<Func<Rating, bool>> BuildUserRatingsForPostsWhereExpression(
            Guid userId, PaginatedList<PostWithUserRatingVm> posts)
        {
            Expression expression = null;

            var rating = Expression.Parameter(typeof(Rating), "rating");
            var postProperty = Expression.Property(rating, "PostId");

            var firstOr = true;
            foreach (var post in posts.Items)
            {
                var postConstant = Expression.Convert(Expression.Constant(post.Id), postProperty.Type);
                var postEquality = Expression.Equal(postProperty, postConstant);

                if (firstOr)
                {
                    firstOr = false;
                    expression = postEquality;
                    continue;
                }

                expression = Expression.Or(expression, postEquality);
            }

            var userProperty = Expression.Property(rating, "UserId");
            var userConstant = Expression.Constant(userId);
            var userEquality = Expression.Equal(userProperty, userConstant);

            expression = Expression.And(expression, userEquality);
            var lambda = Expression.Lambda<Func<Rating, bool>>(expression, rating);

            return lambda;
        }

        public PaginatedList<PostWithUserRatingVm> AddUserRatingsToPosts(List<Rating> ratings, PaginatedList<PostWithUserRatingVm> posts)
        {
            if (ratings.Count == 0) return posts;

            var postDictionary = posts.Items.ToDictionary(x => x.Id, x => x);

            foreach (var rating in ratings)
            {
                if (rating.PostId.HasValue)
                {
                    if (postDictionary.TryGetValue(rating.PostId.Value, out var post))
                    {
                        post.UserRating = _mapper.Map<Rating, RatingVm>(rating);
                    }
                }
            }

            return posts;
        }
    }
}
