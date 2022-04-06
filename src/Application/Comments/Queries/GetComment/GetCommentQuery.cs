using AutoMapper;
using AutoMapper.QueryableExtensions;
using HikingTrailsApi.Application.Common.Interfaces;
using HikingTrailsApi.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HikingTrailsApi.Application.Comments.Queries.GetComment
{
    public class GetCommentQuery : IRequest<Result<CommentVm>>
    {
        public Guid Id { get; set; }
    }

    public class GetCommentQueryHandler : IRequestHandler<GetCommentQuery, Result<CommentVm>>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public GetCommentQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
        }

        public async Task<Result<CommentVm>> Handle(GetCommentQuery request, CancellationToken cancellationToken)
        {
            var commentVm = await _applicationDbContext.Comments
                .AsNoTracking()
                .Include(x => x.User)
                .ProjectTo<CommentVm>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (commentVm == null)
            {
                return Result<CommentVm>.NotFound("Id", "Nepavyko surasti komentaro");  //404
            }

            return Result<CommentVm>.Success(commentVm);    //200
        }
    }
}
