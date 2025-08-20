using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seeder;

public static class UserSeeder
{
    public static async Task SeedAsync(UserManager<User> _userManager)
    {
        var usersCount = await _userManager.Users.CountAsync();
        if (usersCount <= 0)
        {
            var defaultuser = new User()
            {
                UserName = "AbuBassam",
                Email = "programmerabubassam@gmail.com",
                EmailConfirmed = true,
                RoleID = 1

            };
            await _userManager.CreateAsync(defaultuser, "Test123456.");

            var testUsers = new List<User>
            {
                new User { UserName = "TestUser1", Email = "testuser1@gmail.com", EmailConfirmed = true, RoleID = 2 },
                new User { UserName = "TestUser2", Email = "testuser2@gmail.com", EmailConfirmed = true, RoleID = 2 },
                new User { UserName = "TestUser3", Email = "testuser3@gmail.com", EmailConfirmed = true, RoleID = 2 },
                new User { UserName = "TestUser4", Email = "testuser4@gmail.com", EmailConfirmed = true, RoleID = 2 },
                new User { UserName = "TestUser5", Email = "testuser5@gmail.com", EmailConfirmed = true, RoleID = 2 }
            };

            foreach (var user in testUsers)
            {
                await _userManager.CreateAsync(user, "Test123456.");
            }

        }
    }
}