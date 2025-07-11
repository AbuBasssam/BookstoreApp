﻿using Domain.AppMetaData;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seeder;

public static class RoleSeeder
{
    public static async Task SeedAsync(RoleManager<Role> _roleManager)
    {
        var rolesCount = await _roleManager.Roles.CountAsync();
        if (rolesCount <= 0)
        {

            await _roleManager.CreateAsync(new Role()
            {
                Name = Roles.Admin
            });
            await _roleManager.CreateAsync(new Role()
            {
                Name = Roles.Member
            });
        }
    }

}
