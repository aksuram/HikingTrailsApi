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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace HikingTrailsApi.Application.Comments.Queries.GetComments
{
    public class GetCommentsQuery : IRequest<Result<FormattedCommentListVm>>
    {
        public Guid PostId { get; set; }
    }

    public class GetCommentsQueryHandler : IRequestHandler<GetCommentsQuery, Result<FormattedCommentListVm>>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetCommentsQueryHandler(IApplicationDbContext applicationDbContext,
            IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<FormattedCommentListVm>> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
        {
            //TODO: Check if WHERE clause is executed in the database and not the program itself
            var comments = await _applicationDbContext.Comments
                .AsNoTracking()
                .Include(x => x.User)
                .ThenInclude(x => x.Images)
                .Where(x => x.PostId == request.PostId)
                .OrderBy(x => x.CreationDate)
                .ProjectTo<CommentWithUserRatingVm>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            if (comments.Count == 0)
            {
                return Result<FormattedCommentListVm>.NotFound("PostId", "Nepavyko surasti duoto įrašo komentarų");  //404
            }

            if (_httpContextAccessor?.HttpContext?.User?.Identity.IsAuthenticated ?? false)
            {
                var userId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "id");
                var whereExpression = BuildUserRatingsForCommentsWhereExpression(new Guid(userId.Value), comments);
                var ratings = await _applicationDbContext.Ratings
                    .AsNoTracking()
                    .Where(whereExpression)
                    .ToListAsync();

                comments = AddUserRatingsToComments(ratings, comments);
            }

            var formattedComments = FormatCommentStructure(comments);

            return Result<FormattedCommentListVm>.Success(formattedComments);  //200
        }

        //TODO: Could probably figure out a proper sql query rather than this
        public Expression<Func<Rating, bool>> BuildUserRatingsForCommentsWhereExpression(
            Guid userId, List<CommentWithUserRatingVm> comments)
        {
            Expression expression = null;

            var rating = Expression.Parameter(typeof(Rating), "rating");
            var commentProperty = Expression.Property(rating, "CommentId");

            var firstOr = true;
            foreach (var comment in comments)
            {
                var commentConstant = Expression.Convert(Expression.Constant(comment.Id), commentProperty.Type);
                var commentEquality = Expression.Equal(commentProperty, commentConstant);

                if (firstOr)
                {
                    firstOr = false;
                    expression = commentEquality;
                    continue;
                }

                expression = Expression.Or(expression, commentEquality);
            }

            var userProperty = Expression.Property(rating, "UserId");
            var userConstant = Expression.Constant(userId);
            var userEquality = Expression.Equal(userProperty, userConstant);

            expression = Expression.And(expression, userEquality);
            var lambda = Expression.Lambda<Func<Rating, bool>>(expression, rating);

            return lambda;
        }

        public List<CommentWithUserRatingVm> AddUserRatingsToComments(List<Rating> ratings, List<CommentWithUserRatingVm> comments)
        {
            if (ratings.Count == 0) return comments;

            var commentDictionary = comments.ToDictionary(x => x.Id, x => x);

            foreach (var rating in ratings)
            {
                if (rating.CommentId.HasValue)
                {
                    if (commentDictionary.TryGetValue(rating.CommentId.Value, out var comment))
                    {
                        comment.UserRating = _mapper.Map<Rating, RatingVm>(rating);
                    }
                }
            }

            return comments;
        }

        private FormattedCommentListVm FormatCommentStructure(List<CommentWithUserRatingVm> comments)
        {
            var formatedComments = new FormattedCommentListVm { CommentCount = comments.Count };

            var firstLevelComments = new Dictionary<Guid, CommentWithUserRatingVm>();     //Comments that are not replies
            var secondLevelComments = new List<CommentWithUserRatingVm>();                //Comments that are replies

            //Get first level comments and get second level comments into their own data structures
            foreach (var comment in comments)
            {
                if (comment.ReplyToId.HasValue)
                {
                    secondLevelComments.Add(comment);
                    continue;
                }

                firstLevelComments.Add(comment.Id, comment);
            }

            //Add second level comments to their respective first level comments
            foreach (var secondLevelComment in secondLevelComments)
            {
                var firstLevelComment = firstLevelComments[(Guid)secondLevelComment.ReplyToId];

                if (firstLevelComment.Replies == null) firstLevelComment.Replies = new List<CommentWithUserRatingVm>();

                firstLevelComment.Replies.Add(secondLevelComment);
            }

            formatedComments.Comments = firstLevelComments.Values.ToList();

            return formatedComments;
        }
    }
}
