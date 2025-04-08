using Microsoft.AspNetCore.Identity;
using wedding_planer_ad.Models;

namespace wedding_planer_ad.Data.Seeders
{
    public static class UserRoleSeeder
    {
        public static async Task SeedRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roles = { "Admin", "Planner", "Couple" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }


        public static async Task SeedInitialUsers(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = { "Admin", "Planner", "Couple" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Define users per role
            var usersToSeed = new List<(string email, string role, string firstName, string lastName)>
        {
            ("admin@mail.com", "Admin", "Admin","user"),
            ("planner@mail.com", "Planner", "Planner","user"),
            ("couple@mail.com", "Couple", "Couple","one"),

        };

            foreach (var (email, role, firstName, lastName) in usersToSeed)
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    var newUser = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true,
                        FirstName = firstName,
                        LastName = lastName
                    };

                    var result = await userManager.CreateAsync(newUser, "Password@123");

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newUser, role);
                    }
                    else
                    {
                        // Log or display errors if needed
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine($"{email} - Error: {error.Description}");
                        }
                    }
                }
            }
        }
    }
}