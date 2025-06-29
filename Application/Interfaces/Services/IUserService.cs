using Domain.Entities;

namespace Application.Interfaces;

public interface IUserService
{
    IQueryable<User> GetUserByEmailAsync(string email);
    Task<List<string>> GetUserRolesAsync(User user);
    Task<bool> CheckPasswordAsync(User User, string Password);

}