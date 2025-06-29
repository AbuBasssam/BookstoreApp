using Domain.Entities;

namespace Application.Interfaces;

public interface IRefreshTokenRepository : IGenericRepository<UserRefreshToken, int>
{
}
