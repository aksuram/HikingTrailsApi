using AutoMapper;
using AutoMapper.QueryableExtensions;
using HikingTrailsApi.Application.Common.Interfaces;
using HikingTrailsApi.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HikingTrailsApi.Application.Posts.Queries.GetPost
{
    public class GetPostQuery : IRequest<Result<PostVm>>
    {
        public Guid Id { get; set; }
    }

    public class GetPostQueryHandler : IRequestHandler<GetPostQuery, Result<PostVm>>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public GetPostQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
        }

        public async Task<Result<PostVm>> Handle(GetPostQuery request, CancellationToken cancellationToken)
        {
            var postVm = await _applicationDbContext.Posts
                .AsNoTracking()
                .Include(x => x.User)
                .ProjectTo<PostVm>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (postVm == null)
            {
                return Result<PostVm>.NotFound("Id", "Nepavyko surasti įrašo"); //404
            }

            return Result<PostVm>.Success(postVm); //200
        }
    }
}
