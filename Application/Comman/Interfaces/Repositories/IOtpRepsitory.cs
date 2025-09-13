using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Interfaces;

public interface IOtpRepsitory : IGenericRepository<Otp, int>
{
    IQueryable<Otp> GetLastOtpAsync(string email, enOtpType otpType);

}