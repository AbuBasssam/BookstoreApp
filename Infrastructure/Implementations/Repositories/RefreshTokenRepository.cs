using Application.Interfaces;
using Domain.Entities;

namespace Infrastructure.Repositories;

public class RefreshTokenRepository : GenericRepository<UserRefreshToken, int>, IRefreshTokenRepository
{
    public RefreshTokenRepository(AppDbContext context) : base(context)
    {
    }
    
}