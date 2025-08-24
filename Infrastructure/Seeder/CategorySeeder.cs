using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seeder;

public static class CategorySeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Categories.AnyAsync())
            return;
        var categories = Enum.GetValues(typeof(enCategory))
                             .Cast<enCategory>()
                             .Select(c => new Category
                             {
                                 Id = c,
                                 NameEN = c.ToString(),
                                 NameAR = c switch
                                 {
                                     enCategory.Romance => "رومانسية",
                                     enCategory.Fantasy => "فانتازيا",
                                     enCategory.Mystery => "غموض",
                                     enCategory.Programming => "برمجة",
                                     enCategory.Economics => "إقتصاد",
                                     enCategory.History => "تاريخ",
                                     enCategory.Science => "علوم",
                                     _ => throw new NotImplementedException(),
                                 }
                             }).ToList();
        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();
    }

}
