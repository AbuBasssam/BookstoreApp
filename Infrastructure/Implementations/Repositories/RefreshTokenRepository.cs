using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class RefreshTokenRepository : GenericRepository<UserRefreshToken, int>, IRefreshTokenRepository
{
    public RefreshTokenRepository(AppDbContext context) : base(context)
    {
    }
    public IQueryable<UserRefreshToken> GetActiveSessionTokenByUserId(int userId)
    {
        return _dbSet
            .Where(token => token.UserId == userId
                         && !token.IsRevoked
                         && token.IsUsed
                         && token.RefreshToken == null// mark for Session token
                         && token.ExpiryDate > DateTime.UtcNow);
    }
    public async Task<bool> IsTokenExpired(string acessToken)
    {
        var IsTokenExpired = await _dbSet.AnyAsync
            (
                t => t.AccessToken!.Equals(acessToken)
                && t.IsRevoked
                && DateTime.UtcNow > t.ExpiryDate
            );

        return IsTokenExpired;
    }

}