using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seeder;

public static class PublisherSeeder{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Publishers.AnyAsync())
            return; 
        var publishers = new List<Publisher>
        {
            new Publisher { NameEN = "Penguin Random House", NameAR = "بنغوين راندوم هاوس" },
            new Publisher { NameEN = "Harper Collins", NameAR = "هاربر كولينز" },
            new Publisher { NameEN = "Simon & Schuster", NameAR = "سايمون وشوستر" },
            new Publisher { NameEN = "Hachette Livre", NameAR = "هاشيت ليفر" },
            new Publisher { NameEN = "Macmillan Publishers", NameAR = "ماكميلان للنشر" },
            new Publisher { NameEN = "Scholastic Corporation", NameAR = "سكولاستيك كوربوريشن" },
            new Publisher { NameEN = "Oxford University Press", NameAR = "مطبعة جامعة أكسفورد" },
            new Publisher { NameEN = "Cambridge University Press", NameAR = "مطبعة جامعة كامبريدج" },
            new Publisher { NameEN = "Wiley", NameAR = "وايلي" },
            new Publisher { NameEN = "Springer Nature", NameAR = "سبرينجر نيتشر" }
        };
        await context.Publishers.AddRangeAsync(publishers);
        await context.SaveChangesAsync();
    }
}