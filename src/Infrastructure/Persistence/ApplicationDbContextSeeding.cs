using HikingTrailsApi.Application.Common.Interfaces;
using HikingTrailsApi.Domain.Entities;
using HikingTrailsApi.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HikingTrailsApi.Infrastructure.Persistence
{
    public static class ApplicationDbContextSeeding
    {
        public static async Task SeedData(IServiceScope serviceScope)
        {
            using var applicationDbContext = serviceScope.ServiceProvider
                .GetRequiredService<IApplicationDbContext>();

            var configuration = serviceScope.ServiceProvider
                            .GetRequiredService<IConfiguration>();

            //TODO: Maybe use DbContext.Database.Migrate() as migrations can't be used for the database with this method
            //await ((DbContext)applicationDbContext).Database.EnsureDeletedAsync();
            //await ((DbContext)applicationDbContext).Database.EnsureCreatedAsync();

            await ((DbContext)applicationDbContext).Database.MigrateAsync();

            //If seeding is disabled - return
            if (configuration["Seeding:IsEnabled"].ToLower() != "true") { return; }
            var seedingCategories = GetSeedingCategories(configuration);

            if (seedingCategories.Contains("Users")) await SeedInitialUsers(applicationDbContext);
        }

        private static async Task SeedInitialUsers(IApplicationDbContext applicationDbContext)
        {
            //Inital user list
            var usersToSeed = new List<User>
            {
                new User
                {
                    Email = "admin@admin.lt",
                    Password = BCrypt.Net.BCrypt.HashPassword("Taip1234"),
                    Role = Role.Administrator,
                    FirstName = "Admin",
                    LastName = "",
                    IsEmailConfirmed = true,
                    CreationDate = DateTime.UtcNow
                },
                new User
                {
                    Email = "moderator@moderator.lt",
                    Password = BCrypt.Net.BCrypt.HashPassword("Taip1234"),
                    Role = Role.Moderator,
                    FirstName = "Moderator",
                    LastName = "",
                    IsEmailConfirmed = true,
                    CreationDate = DateTime.UtcNow
                },
                new User
                {
                    Email = "user@user.lt",
                    Password = BCrypt.Net.BCrypt.HashPassword("Taip1234"),
                    Role = Role.User,
                    FirstName = "User",
                    LastName = "",
                    IsEmailConfirmed = true,
                    CreationDate = DateTime.UtcNow
                }
            };

            foreach (var user in usersToSeed)
            {
                applicationDbContext.Users.Add(
                    new User
                    {
                        Email = user.Email,
                        Password = user.Password,
                        Role = user.Role,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        IsEmailConfirmed = user.IsEmailConfirmed,
                        CreationDate = user.CreationDate
                    });
            }

            await applicationDbContext.SaveChangesAsync(CancellationToken.None);
        }

        private static List<string> GetSeedingCategories(IConfiguration configuration)
        {
            var seedingSection = configuration.GetSection("Seeding:Categories");
            var seedingCategories = new List<string>();

            foreach (var section in seedingSection.GetChildren())
            {
                seedingCategories.Add(section.Value);
            }

            return seedingCategories;
        }
    }
}
