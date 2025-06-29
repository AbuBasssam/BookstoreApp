using Domain.Entities;

namespace Application.Interfaces;

public interface IUserService
{
    Task<List<string>> GetUserRolesAsync(User user);
}