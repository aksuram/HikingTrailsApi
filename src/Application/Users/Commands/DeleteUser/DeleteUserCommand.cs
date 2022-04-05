using HikingTrailsApi.Application.Common.Interfaces;
using HikingTrailsApi.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HikingTrailsApi.Application.Users.Commands.DeleteUser
{
    public class DeleteUserCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
    {
        private readonly IApplicationDbContext _applicationDbContext;

        public DeleteUserCommandHandler(IApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _applicationDbContext.Users
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (user == null)
            {
                return Result.NotFound("Id", "Nepavyko surasti naudotojo"); //404
            }

            if (user.IsDeleted)
            {
                return Result.NotFound("Id", "Naudotojas jau užblokuotas"); //404
            }

            user.IsDeleted = true;
            await _applicationDbContext.SaveChangesAsync(cancellationToken);

            return Result.NoContent();  //204
        }
    }
}
