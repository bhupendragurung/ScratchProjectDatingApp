using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScratchProjectDatingApp.Entity;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ScratchProjectDatingApp.Data
{
    public class Seed
    {
        public static async Task ClearConnections (DataContext context)
        {
            context.Connections.RemoveRange(context.Connections);
            await context.SaveChangesAsync();
        }
        public static async Task SeedUser(UserManager<AppUser> userManager,RoleManager<AppRole> roleManager)
        {
            if (await userManager.Users.AnyAsync()) return;

            var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);
            var roles = new List<AppRole> {
                new AppRole{Name="Member"},
                new AppRole{Name="Admin"},
                new AppRole{Name="Moderator"},
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }
            foreach(var user in users)
            {
              
                user.UserName=user.UserName.ToLower();

                user.DateOfBirth=DateTime.SpecifyKind(user.DateOfBirth,DateTimeKind.Utc);
                user.Created=DateTime.SpecifyKind(user.Created,DateTimeKind.Utc);
                user.LastActive=DateTime.SpecifyKind(user.LastActive,DateTimeKind.Utc);
               await userManager.CreateAsync(user,"Pa$$w0rd");
                await userManager.AddToRoleAsync(user, "Member");
            }

            var admin = new AppUser
            {
                UserName = "admin",
            };
            await userManager.CreateAsync(admin, "Pa$$w0rd");
            await userManager.AddToRoleAsync(admin, "Admin" );
            await userManager.AddToRoleAsync(admin,"Moderator");

        } 
    }
}
