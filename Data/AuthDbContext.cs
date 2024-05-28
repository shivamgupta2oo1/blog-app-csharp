using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Bloggie.Web.Data
{
    public class AuthDbContext : IdentityDbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var adminRoleId = "1d4c38a7-4e58-4b2a-8a8e-9d9728e0c1e3";
            var superAdminRoleId = "c5f9cfe8-3c3a-4c9a-a5f5-9f8e349a2d1b";
            var userRoleId = "b8e62e84-2e7b-47b2-9fdf-d7b0b4b2a8b2";

            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    Id = adminRoleId,
                    ConcurrencyStamp = adminRoleId,
                },
                new IdentityRole
                {
                    Name = "SuperAdmin",
                    NormalizedName = "SUPERADMIN",
                    Id = superAdminRoleId,
                    ConcurrencyStamp = superAdminRoleId,
                },
                new IdentityRole
                {
                    Name = "User",
                    NormalizedName = "USER",
                    Id = userRoleId,
                    ConcurrencyStamp = userRoleId,
                },
            };

            builder.Entity<IdentityRole>().HasData(roles);

            var superAdminId = "e02f72d1-620d-4b88-bb73-dcd490b6e1d4";

            var superAdminUser = new IdentityUser
            {
                UserName = "superadmin@bloggie.com",
                Email = "superadmin@bloggie.com",
                NormalizedEmail = "SUPERADMIN@BLOGGIE.COM",
                NormalizedUserName = "SUPERADMIN@BLOGGIE.COM",
                Id = superAdminId,
            };

            superAdminUser.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(superAdminUser, "Superadmin@123");

            builder.Entity<IdentityUser>().HasData(superAdminUser);

            var superAdminRoles = new List<IdentityUserRole<string>>
            {
                new IdentityUserRole<string>
                {
                    RoleId = adminRoleId,
                    UserId = superAdminId
                },
                new IdentityUserRole<string>
                {
                    RoleId = superAdminRoleId,
                    UserId = superAdminId
                },
                new IdentityUserRole<string>
                {
                    RoleId = userRoleId,
                    UserId = superAdminId
                },
            };

            builder.Entity<IdentityUserRole<string>>().HasData(superAdminRoles);
        }
    }
}
