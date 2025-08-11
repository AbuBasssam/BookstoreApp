using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seeder;
public class SystemSettingsSeeder
{
    public static async Task SeedAsync(AppDbContext _context)
    {
        if (!_context.Settings.Any())
        {

            await _context.Database.ExecuteSqlRawAsync(
           @"INSERT INTO SystemSettings (MaxLoanDays,MaxRenewals,RenewalExtensionDays,FinePerDay, MaxLoansPerMember,ReservationExpiryDays,PickupExpiryHours) 
              VALUES (30,2,7,5,5,1,48)");

        }

    }
}
