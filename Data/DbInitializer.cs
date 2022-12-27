using IdentityServerWeb.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityServerWeb.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context, UserManager<Account> userManager)
        {
            
            // Check if there are any users in the database
            if (!userManager.Users.Any())
            {
                //a hasher to hash the password before seeding the user to the db
                var hasher = new PasswordHasher<Account>();
                var user = new Account
                {
                    UserName = "alvin",
                    Email = "alvinmaung@gmail.com"
                };

                // No need saveChangesAsync since CreateAsync does it for us
                await userManager.CreateAsync(user, "Pa$$w0rd");
            }
        }
    }
}