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


        }
    }
}