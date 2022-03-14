using HikingTrailsApi.Application.Common.Interfaces;
using HikingTrailsApi.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace HikingTrailsApi.Application.Common.Behaviours
{
    public class EventLoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IEventLoggable, IRequest<TResponse>
    {
        private readonly IApplicationDbContextFactory<IApplicationDbContext> _applicationDbContextFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDateTime _dateTime;

        public EventLoggingBehaviour(IHttpContextAccessor httpContextAccessor, IDateTime dateTime,
            IApplicationDbContextFactory<IApplicationDbContext> applicationDbContextFactory)
        {
            _applicationDbContextFactory = applicationDbContextFactory;
            _httpContextAccessor = httpContextAccessor;
            _dateTime = dateTime;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var response = await next();

            var eventMessage = request.FormEventMessage();

            using var applicationDbContext = _applicationDbContextFactory.CreateDbContext();

            if (!string.IsNullOrWhiteSpace(eventMessage))
            {
                applicationDbContext.Events.Add(new Event()
                {
                    Description = eventMessage,
                    CreationDate = _dateTime.Now,
                    UserId = Guid.Parse(_httpContextAccessor?.HttpContext?.User?
                        .FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty)
                });

                await applicationDbContext.SaveChangesAsync(cancellationToken);
            }

            return response;
        }
    }
}
