using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Services
{
    public class RoleConstructor
    {
        public static async Task PhanQuen(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Users>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<RoleConstructor>>();

            try
            {
                logger.LogInformation("Ensuring the database is created");
                await context.Database.EnsureCreatedAsync();

                //Them role
                logger.LogInformation("Them Role");
                await AddRoleAsync(roleManager, "Admin");
                await AddRoleAsync(roleManager, "Manager");
                await AddRoleAsync(roleManager, "User");
                await AddRoleAsync(roleManager, "Staff");

                //Khoi tao 1 tai khoan admin
                logger.LogInformation("Them 1 tai khoan admin");
                var adminEmail = "admin123@gmail.com";
                if (await userManager.FindByEmailAsync(adminEmail) == null)
                {
                    var adminUser = new Admin
                    {
                        Name = "Admin1",
                        Gender = 1,
                        Email = adminEmail,
                        NormalizedEmail = adminEmail.ToUpper(),
                        NormalizedUserName = adminEmail.ToUpper(),
                        EmailConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        UserName = adminEmail,
                        DateOfBirth = new DateTime(1990, 1, 1),
                        PhoneNumber = "0326635159"
                    };

                    var result = await userManager.CreateAsync(adminUser, "Admin@123");
                    if (result.Succeeded)
                    {
                        logger.LogInformation("Them user vua tao co role admin");
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                    else
                    {
                        logger.LogError("Tao tai khoan admin that bai!!!{Error}", string.Join(", ", result.Errors.Select(s => s.Description)));
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Co loi khi tao quyen {ex}");
            }
        }

        private static async Task AddRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (!result.Succeeded)
                {
                    throw new Exception($"Loi khoi tao role '{roleName}': {string.Join(", ", result.Errors.Select(s => s.Description))}");
                }

            }
        }
    }
}
