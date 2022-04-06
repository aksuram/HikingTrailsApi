using HikingTrailsApi.Application.Common.Interfaces;
using HikingTrailsApi.Application.Common.Models;
using HikingTrailsApi.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace HikingTrailsApi.Application.Posts.Commands.DeletePost
{
    public class DeletePostCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }

    public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand, Result>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeletePostCommandHandler(IApplicationDbContext applicationDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _applicationDbContext = applicationDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result> Handle(DeletePostCommand request, CancellationToken cancellationToken)
        {
            var post = await _applicationDbContext.Posts
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (post == null)
            {
                return Result.NotFound("Id", "Nepavyko surasti įrašo"); //404
            }

            var userRole = _httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);
            Guid.TryParse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("id"), out var userId);

            if (userId != post.UserId && userRole != Role.Administrator.ToString())
            {
                return Result.Forbidden("Authorization", "Naudotojas negali trinti kitų naudotojų įrašų");  //403
            }

            if (post.IsDeleted)
            {
                return Result.NotFound("Id", "Įrašas jau ištrintas");   //404
            }

            post.IsDeleted = true;
            await _applicationDbContext.SaveChangesAsync(cancellationToken);

            return Result.NoContent();  //204
        }
    }
}
