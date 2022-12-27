using IdentityServerWeb.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityServerWeb.Data
{
    // No need to add a DbSet for Account in here because we're using to ASP.NET identity (IdentityDbContext)
    // https://github.com/dotnet/EntityFramework.Docs/blob/main/samples/core/Modeling/DataSeeding/DataSeedingContext.cs
    public class ApplicationDbContext : IdentityDbContext<Account>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // this.SeedUsers(builder);
        }

        // private void SeedUsers(ModelBuilder builder)
        // {
        //     //a hasher to hash the password before seeding the user to the db
        //     var hasher = new PasswordHasher<Account>();
        //     var user = new Account
        //     {
        //         UserName = "alvin",
        //         Email = "alvinmaung@gmail.com",
        //     };
        //     var hashed = hasher.HashPassword(user, "Pa$$w0rd");
        //     user.PasswordHash = hashed;

        //     builder.Entity<Account>().HasData(user);
        // }
    }
}