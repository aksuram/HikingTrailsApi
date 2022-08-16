using AutoMapper;
using AutoMapper.QueryableExtensions;
using HikingTrailsApi.Application.Common.Interfaces;
using HikingTrailsApi.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HikingTrailsApi.Application.Users.Queries.GetUser
{
    public class GetUserQuery : IRequest<Result<UserVm>>
    {
        public Guid Id { get; set; }
    }

    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, Result<UserVm>>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public GetUserQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
        }

        public async Task<Result<UserVm>> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var userVm = await _applicationDbContext.Users
                .AsNoTracking()
                .ProjectTo<UserVm>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (userVm == null)
            {
                return Result<UserVm>.NotFound("Id", "Nepavyko surasti naudotojo"); //404
            }

            return Result<UserVm>.Success(userVm); //200
        }
    }
}
