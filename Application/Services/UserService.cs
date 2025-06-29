using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.Services;

public class UserService : IUserService
{
    #region Fields
    private readonly UserManager<User> _userManager;
    #endregion

    public UserService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    //Role Manger part
    public async Task<List<string>> GetUserRolesAsync(User User)
    {
        return (await _userManager.GetRolesAsync(User)).ToList();
    }
}