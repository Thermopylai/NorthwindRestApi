using Microsoft.AspNetCore.Identity;
using NorthwindRestApi.Models.Identity;

namespace NorthwindRestApi.Data
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            const string adminUserName = "admin";
            const string adminEmail = "admin@northwind.local";
            const string adminPassword = "admin123";

            var admin = await userManager.FindByNameAsync(adminUserName);

            var adminCreated = false;

            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminUserName,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, adminPassword);

                if (!result.Succeeded)
                {
                    var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Admin user creation failed: {errors}");
                }

                adminCreated = true;
            }

            // Only enforce the default roles at creation time.
            // Otherwise, manual role removals will be re-added on every restart.
            if (adminCreated)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
                await userManager.AddToRoleAsync(admin, "User");
            }
        }
    }
}
