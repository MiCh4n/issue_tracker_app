using System;
using System.Linq;
using System.Threading.Tasks;
using issue_tracker.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace issue_tracker.Models
{
    public class AdministratorAndRoles
    {
        private ApplicationDbContext _context;

        public AdministratorAndRoles(ApplicationDbContext context)
        {
            _context = context;
        }

        public async void SeedAdminUser()
        {
            var user = new ApplicationUser
            {
                UserName = "Admin@issue-tracker.com",
                NormalizedUserName = "admin@issue-tracker.com",
                Email = "Admin@issue-tracker.com",
                NormalizedEmail = "admin@issue-tracker.com",
                SecurityStamp = Guid.NewGuid().ToString()
            };
            
            if (!_context.Roles.Any(r => r.Name == "admin"))
            {
                var roleStore = new RoleStore<IdentityRole>(_context);
                await roleStore.CreateAsync(new IdentityRole {Name = "admin", NormalizedName = "admin"});
            }

            if (!_context.Users.Any(u => u.UserName == user.UserName))
            {
                var password = new PasswordHasher<ApplicationUser>();
                var hashed = password.HashPassword(user, "pass");
                user.PasswordHash = hashed;
                var userStore = new UserStore<ApplicationUser>(_context);
                await userStore.CreateAsync(user);
                await userStore.AddToRoleAsync(user, "admin");
            }
            await _context.SaveChangesAsync();
        }
    }
}