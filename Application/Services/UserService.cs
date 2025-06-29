using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.Services;

public class UserService : IUserService
{
    #region Fields
    private readonly UserManager<User> _userManager;
    #endregion

    #region Constructor(s)
    public UserService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }
    #endregion

    #region Action Method(s)

    //User Manager part
    public IQueryable<User> GetUserByEmailAsync(string email)
    {
        return _userManager.Users.Where(x => x.Email!.ToLower().Equals(email.ToLower()));
    }

    public async Task<bool> CheckPasswordAsync(User User, string Password)
    {
        return await _userManager.CheckPasswordAsync(User, Password);
    }

    //Role Manger part
    public async Task<List<string>> GetUserRolesAsync(User User)
    {
        return (await _userManager.GetRolesAsync(User)).ToList();
    }

    #endregion


}