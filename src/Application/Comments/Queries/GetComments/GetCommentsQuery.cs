using AutoMapper;
using AutoMapper.QueryableExtensions;
using HikingTrailsApi.Application.Common.Interfaces;
using HikingTrailsApi.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public GetCommentsQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
        }

        public async Task<Result<FormattedCommentListVm>> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
        {
            //TODO: Check if WHERE clause is done in the database and not in the program
            var formattedCommentVmList = await _applicationDbContext.Comments
                .AsNoTracking()
                .Include(x => x.User)
                .Where(x => x.PostId == request.PostId)
                .OrderBy(x => x.CreationDate)
                .ProjectTo<FormattedCommentVm>(_mapper.ConfigurationProvider)
                .ToListAsync();

            if (formattedCommentVmList.Count == 0)
            {
                return Result<FormattedCommentListVm>.NotFound("PostId", "Nepavyko surasti duoto įrašo komentarų");  //404
            }

            var formattedComments = FormatCommentStructure(formattedCommentVmList);

            return Result<FormattedCommentListVm>.Success(formattedComments);  //200
        }

        private FormattedCommentListVm FormatCommentStructure(List<FormattedCommentVm> comments)
        {
            var formatedComments = new FormattedCommentListVm { CommentCount = comments.Count };

            var firstLevelComments = new Dictionary<Guid, FormattedCommentVm>();     //Comments that are not replies
            var secondLevelComments = new List<FormattedCommentVm>();                //Comments that are replies

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

                if (firstLevelComment.Replies == null) firstLevelComment.Replies = new List<FormattedCommentVm>();

                firstLevelComment.Replies.Add(secondLevelComment);
            }

            formatedComments.Comments = firstLevelComments.Values.ToList();

            return formatedComments;
        }
    }
}
