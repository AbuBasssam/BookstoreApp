using Domain.Entities;
using Domain.Enums;
using Infrastructure;
using Infrastructure.Repositories;
using Interfaces;

namespace Implementations;

public class OtpRepository : GenericRepository<Otp, int>, IOtpRepsitory
{
    public OtpRepository(AppDbContext context) : base(context)
    {
    }

    public IQueryable<Otp> GetLastOtpAsync(string Email, enOtpType otpType)
    {
        return _dbSet
            .Where(o => o.User.Email == Email && o.Type == otpType)
            .OrderByDescending(o => o.ExpirationTime);

    }
}

