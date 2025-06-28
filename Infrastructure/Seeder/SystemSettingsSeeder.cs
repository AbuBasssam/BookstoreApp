using Domain.Entities;

namespace Infrastructure.Seeder;
public class SystemSettingsSeeder
{
    public static async Task SeedAsync(AppDbContext _context)
    {
        if (!_context.Settings.Any())
        {

            await _context.Settings.AddAsync(new SystemSettings());

            await _context.SaveChangesAsync();

        }

    }
}
