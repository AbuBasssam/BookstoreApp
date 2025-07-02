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

    public IQueryable<User> GetUserById(int Id)
    {
        return _userManager.Users.Where(x => x.Id == Id);
    }
    public IQueryable<User> GetUserByEmailAsync(string email)
    {
        return _userManager.Users.Where(x => x.Email!.ToLower().Equals(email.ToLower()));
    }

    public Task<IdentityResult> CreateUserAsync(User user, string password)
    {
        return _userManager.CreateAsync(user, password);
    }

    public async Task<bool> CheckPasswordAsync(User User, string Password)
    {
        return await _userManager.CheckPasswordAsync(User, Password);
    }

    #endregion


}