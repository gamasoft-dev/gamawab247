using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Identities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Data.DbContext;
public static class DbInitializer
{
    public static async Task SeedRoleData(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();

        var roles = new List<Role>
        {
            new Role
            {
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                Name = ERole.ADMIN.ToString()
            },
            new Role
            {
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                Name = ERole.USER.ToString()
            },
            new Role
            {
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                Name = ERole.SUPERADMIN.ToString()
            }
        };

        if (!roleManager.Roles.Any())
        {
            foreach (var role in roles)
            {
                if(!await roleManager.RoleExistsAsync(role.Name))
                    await roleManager.CreateAsync(role);
            }
        }
    }

    public static async Task SeedSuperUser(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

        var superUser = new User
        {
            Status = EUserStatus.ACTIVE.ToString(),
            FirstName = "Super user",
            LastName = "Chris Dan Gbengs",
            Verified = true,
            UpdatedAt = DateTime.Now,
            Email = "support@gamasoft.com",
            PhoneNumber = "070Gamasoft"
        };
        superUser.UserName = superUser.Email;
        superUser.Verified = true;
        
        string password = "Password@123";

        User user = await userManager.FindByEmailAsync(superUser.Email);
        if (user is null)
        {
          var userCreationResult = await userManager.CreateAsync(superUser, password);
          if (!userCreationResult.Succeeded)
              throw new Exception(userCreationResult.Errors.ToString());
        }
        else
        {
            superUser = user;
        }

        var roles = new List<string>()
        {
            new(ERole.SUPERADMIN.ToString()),
            new(ERole.USER.ToString()),
            new(ERole.ADMIN.ToString()),
        };
        foreach (var role in roles)
        {
            if(!await userManager.IsInRoleAsync(superUser, role)) 
                await userManager.AddToRoleAsync(superUser, role);
        }
    }
}