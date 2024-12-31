using Microsoft.AspNetCore.Identity;

namespace BitsionTest.API.Domain.Contracts
{
    public static class RoleSeeder
    {
        // con esta clase vamos a gestionar los roles de los usuarios usando Identity
        public static async Task SeedRolesAndAdminUser(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // Creamos los dos roles
            string[] roles = { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Crear un usuario administrador si no existe
            var adminUser = await userManager.FindByEmailAsync("admin@bitsion.com");
            if (adminUser == null)
            {
                var user = new IdentityUser
                {
                    Email = "admin@bitsion.com",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, "Admin123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }
}
