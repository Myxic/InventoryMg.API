using InventoryMg.DAL.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryMg.DAL.Seed
{
    public class SeedData
    {
        public static async Task EnsurePopulatedAsync(IApplicationBuilder app)
        {
            ApplicationDbContext appDbContext = app.ApplicationServices.CreateScope().ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

            using var scope = app.ApplicationServices.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserProfile>>();

            if (!await roleManager.RoleExistsAsync("admin"))
            {
                await roleManager.CreateAsync(new AppRole("admin"));
            }

            if (!await appDbContext.Users.AnyAsync())
            {
                var result = await userManager.CreateAsync(AddAdmin(), "AdminPass12345@");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(AddAdmin(), "admin");
                }
                // await appDbContext.Users.AddAsync(AddAdmin());
                await appDbContext.SaveChangesAsync();
            }
        }

        private static UserProfile AddAdmin()
        {
            return new UserProfile()
            {
                FullName = "Admin One",
                Email = "admin@domain.com",
                Phone = "2233445566"
            };
        }




    }
}
