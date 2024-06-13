using Bloggie.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bloggie.Web.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AuthDbContext authDbContext;

        public UserRepository(AuthDbContext authDbContext)
        {
            this.authDbContext = authDbContext;
        }

        public async Task<IEnumerable<IdentityUser>> GetAll(string? searchQuery)
        {
            var users = await authDbContext.Users.ToListAsync();

            // Remove the super admin user
            var superAdminUser = users.FirstOrDefault(x => x.Email == "superadmin@bloggie.com");
            if (superAdminUser != null)
            {
                users.Remove(superAdminUser);
            }

            // Filter by search query if provided
            if (!string.IsNullOrEmpty(searchQuery))
            {
                users = users.Where(user =>
                    user.UserName.Contains(searchQuery, System.StringComparison.OrdinalIgnoreCase) ||
                    user.Email.Contains(searchQuery, System.StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return users;
        }
    }
}
