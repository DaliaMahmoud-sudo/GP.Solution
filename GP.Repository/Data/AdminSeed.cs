using GP.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Repository.Data
{
    public static class AdminSeed
    {
        public static async Task SeedAdminAsync(UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var User = new AppUser()
                {
                    FirstName = "Dalia",
                    LastName = "Mahmoud",
                    Email = "dalia@gmail.com",
                    UserName = "dalia",
                    PhoneNumber = "12345"
                };
                await userManager.CreateAsync(User, "Pa$$w0rd");
            }
        }

    }
}
