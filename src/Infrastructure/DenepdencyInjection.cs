using HikingTrailsApi.Application.Common.Identity;
using HikingTrailsApi.Application.Common.Interfaces;
using HikingTrailsApi.Domain.Entities;
using HikingTrailsApi.Infrastructure.Persistence;
using HikingTrailsApi.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace HikingTrailsApi.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            //TODO: Check how LazyLoading works
            //DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseLazyLoadingProxies().UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    x => x.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());

            //DbContext factory
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseLazyLoadingProxies().UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                x => x.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));

            services.AddSingleton<IApplicationDbContextFactory<IApplicationDbContext>>(
                new ApplicationDbContextFactory(optionsBuilder.Options));

            //Other
            services.AddTransient<IDateTime, DateTimeService>();

            return services;
        }

        public static IServiceCollection AddApplicationIdentity(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddIdentity<User, UserRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            serviceCollection.Configure<IdentityOptions>(options =>
            {
                options.Lockout.AllowedForNewUsers = false;
                options.Password.RequireNonAlphanumeric = false;
            });

            //serviceCollection.ConfigureApplicationCookie(options =>
            //{
            //    options.ExpireTimeSpan = TimeSpan.FromDays(14);
            //    options.AccessDeniedPath = $"/";
            //    options.LoginPath = $"/login";
            //    options.LogoutPath = $"/logout";
            //});

            var jwtSettings = new JwtSettings()
            {
                Secret = configuration.GetSection("JwtSettings")["Secret"]
            };
            serviceCollection.AddSingleton(jwtSettings);

            serviceCollection
                .AddAuthentication()
                .AddJwtBearer(x =>
                {
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        RequireExpirationTime = true,
                        ValidateLifetime = true
                    };
                });

            serviceCollection.AddHttpContextAccessor();

            return serviceCollection;
        }
    }
}
