using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using OrderTaker.Data;

namespace OrderTaker.Models
{
    public static class IdentitySeedData
    {
        private const string adminUserEmail = "admin@admin.com";
        private const string adminPassword = "@dmin1234";


        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var context = new AppIdentityDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<AppIdentityDbContext>>());

            var user = new User
            {
                Email = adminUserEmail,
                UserName = adminUserEmail,
                FirstName = "Paul Aaron",
                LastName = "Jamila",
                EmailConfirmed = true
            };

            var userID = await EnsureUser(serviceProvider, user, adminPassword);


            await SeedData.Initialize(serviceProvider, user);
        }

        private static async Task<string> EnsureUser(
            IServiceProvider serviceProvider,
            User user,
            string userPW)
        {
            var userManager = serviceProvider.GetService<UserManager<User>>();
            var existingUser = await userManager.FindByEmailAsync(user.Email);
            if (existingUser == null)
            {
                await userManager.CreateAsync(user, userPW);
            }

            if (user == null)
            {
                throw new Exception("The password is probably not strong enough!");
            }

            return user.Id;
        }
    }
}
