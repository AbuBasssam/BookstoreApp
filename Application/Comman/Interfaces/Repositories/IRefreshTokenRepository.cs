using Domain.Entities;

namespace Application.Interfaces;

public interface IRefreshTokenRepository : IGenericRepository<UserRefreshToken, int>
{
    IQueryable<UserRefreshToken> GetActiveSessionTokenByUserId(int userId);
    Task<bool> IsTokenExpired(string acessToken);
}
