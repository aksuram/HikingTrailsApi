using HikingTrailsApi.Application.Common.Interfaces;
using HikingTrailsApi.Domain.Entities;
using HikingTrailsApi.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HikingTrailsApi.Infrastructure.Persistence
{
    public static class ApplicationDbContextSeeding
    {
        public static async Task SeedData(IServiceScope serviceScope)
        {
            using var applicationDbContext = serviceScope.ServiceProvider
                .GetRequiredService<IApplicationDbContext>();

            //TODO: Maybe use DbContext.Database.Migrate() as migrations can't be used for the database with this method
            //await ((DbContext) applicationDbContext).Database.EnsureDeletedAsync();
            await ((DbContext)applicationDbContext).Database.EnsureCreatedAsync();

            //var configuration = serviceScope.ServiceProvider
            //    .GetRequiredService<IConfiguration>();

            ////If seeding is disabled - return
            //if (configuration["Seeding:IsEnabled"] != "True") { return; }

            //var seedingCategories = GetSeedingCategories(configuration);

            ////Seeding based on categories which were written in the configuration file
            //if (seedingCategories.Contains("Roles"))
            //{
            //    var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<UserRole>>();
            //    await SeedUserRoles(roleManager);
            //}

            //if (seedingCategories.Contains("Users"))
            //{
            //    var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
            //    await SeedInitialUsers(userManager);
            //}
        }

        //private static async Task SeedUserRoles(RoleManager<UserRole> roleManager)
        //{
        //    foreach (var role in Enum.GetNames<Role>())
        //    {
        //        if (await roleManager.FindByNameAsync(role) == null)
        //        {
        //            await roleManager.CreateAsync(new UserRole()
        //            {
        //                Name = role
        //            });
        //        }
        //    }
        //}

        //private static async Task SeedInitialUsers(UserManager<User> userManager)
        //{
        //    //Inital user list
        //    List<(string Username, string Password, string FirstName, string LastName, Role Role)> userDetailsList = new()
        //    {
        //        ("Administrator", "Administrator1!", "Administrator", "Administrator", Role.Administrator),
        //        ("Moderator", "Moderator1!", "Moderator", "Moderator", Role.Moderator),
        //        ("User", "User1!", "User", "User", Role.User)
        //    };

        //    foreach (var userDetails in userDetailsList)
        //    {
        //        if (await userManager.FindByNameAsync(userDetails.Username) == null)
        //        {
        //            //TODO: Check if everything is added
        //            var user = new User()
        //            {
        //                UserName = userDetails.Username,
        //                Role = userDetails.Role,
        //                FirstName = userDetails.FirstName,
        //                LastName = userDetails.LastName
        //            };

        //            await userManager.CreateAsync(user, userDetails.Password);

        //            await userManager.AddToRoleAsync(user, user.Role.ToString());
        //        }
        //    }
        //}

        //private static List<string> GetSeedingCategories(IConfiguration configuration)
        //{
        //    var seedingSection = configuration.GetSection("Seeding:Categories");
        //    var seedingCategories = new List<string>();

        //    foreach (var section in seedingSection.GetChildren())
        //    {
        //        seedingCategories.Add(section.Value);
        //    }

        //    return seedingCategories;
        //}
    }
}
