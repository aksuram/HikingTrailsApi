using FluentValidation;
using HikingTrailsApi.Application.Common.Behaviours;
using HikingTrailsApi.Application.Common.Identity;
using HikingTrailsApi.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HikingTrailsApi.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());

            //TODO: Check if needed
            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

            //TODO: Check Transient and Scoped meaning
            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(EventLoggingBehaviour<,>));

            services.AddScoped<IApplicationIdentityManager, ApplicationIdentityManager>();

            return services;
        }
    }
}
