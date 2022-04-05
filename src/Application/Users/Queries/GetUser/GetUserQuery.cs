using HikingTrailsApi.Application.Common.Interfaces;
using HikingTrailsApi.Application.Common.Mappings;
using HikingTrailsApi.Application.Models;
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

        public GetUserQueryHandler(IApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<Result<UserVm>> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _applicationDbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (user == null)
            {
                return Result<UserVm>.NotFound("Id", "Nepavyko surasti naudotojo"); //404
            }

            return Result<UserVm>.Success(UserMappings.ToUserVm(user)); //200
        }
    }
}
