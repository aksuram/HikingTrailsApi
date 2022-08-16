using AutoMapper;
using HikingTrailsApi.Application.Common.Interfaces;
using HikingTrailsApi.Application.Common.Models;
using HikingTrailsApi.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace HikingTrailsApi.Application.Users.Commands.UpdateUser
{
    public class UpdateUserCommand : IRequest<Result<UserVm>>
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<UserVm>>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateUserCommandHandler(IApplicationDbContext applicationDbContext,
            IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        //TODO: User change password endpoint
        //TODO: Admin user change endpoint
        //TODO: Email confirmation endpoint
        public async Task<Result<UserVm>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var updateUserCommandValidator = new UpdateUserCommandValidator();
            var validationResult = await updateUserCommandValidator
                .ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Result<UserVm>.BadRequest(validationResult.Errors.Select(x =>
                    new FieldError(x.PropertyName, x.ErrorMessage)));   //400
            }

            var user = await _applicationDbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (user == null)
            {
                return Result<UserVm>.NotFound("Id", "Nepavyko surasti naudotojo"); //404
            }

            Guid.TryParse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("id"), out var userId);
            if (request.Id != userId)
            {
                return Result<UserVm>.Forbidden("Authorization", "Naudotojas negali keisti kitų naudotojų duomenų");    //403
            }

            if (user.IsDeleted)
            {
                return Result<UserVm>.NotFound("User deleted", "Naudotojas yra užblokuotas"); //404
            }

            if (!user.IsEmailConfirmed)
            {
                return Result<UserVm>.NotFound("Email unconfirmed", "Naudotojas yra nepatvirtinęs el. pašto adreso");  //404
            }

            await _applicationDbContext.Users
                .Where(x => x.Id == request.Id)
                .UpdateAsync(x =>
                    new User
                    {
                        FirstName = request.FirstName,
                        LastName = request.LastName
                    },
                cancellationToken);

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;

            return Result<UserVm>.Success(_mapper.Map<User, UserVm>(user)); //200
        }
    }
}
