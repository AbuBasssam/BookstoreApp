using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.Interfaces;

public interface IUserService
{
    IQueryable<User> GetUserById(int Id);

    IQueryable<User> GetUserByEmailAsync(string email);
    Task<IdentityResult> CreateUserAsync(User user, string password);

    Task<bool> CheckPasswordAsync(User User, string Password);

}