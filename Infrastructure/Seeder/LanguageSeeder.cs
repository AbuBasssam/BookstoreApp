using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seeder;

public static class LanguageSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Languages.AnyAsync())
            return; 

        var languages = new List<Language>
            {
                new Language { Code = "ar", NameAR = "العربية",    NameEN = "Arabic" },
                new Language { Code = "en", NameAR = "الإنجليزية",  NameEN = "English" },
                new Language { Code = "fr", NameAR = "الفرنسية",   NameEN = "French" },
                new Language { Code = "es", NameAR = "الإسبانية",   NameEN = "Spanish" },
                new Language { Code = "de", NameAR = "الألمانية",   NameEN = "German" },
                new Language { Code = "it", NameAR = "الإيطالية",   NameEN = "Italian" },
                new Language { Code = "ru", NameAR = "الروسية",    NameEN = "Russian" },
                new Language { Code = "zh", NameAR = "الصينية",    NameEN = "Chinese" },
                new Language { Code = "ja", NameAR = "اليابانية",  NameEN = "Japanese" },
                new Language { Code = "ko", NameAR = "الكورية",    NameEN = "Korean" },
                new Language { Code = "pt", NameAR = "البرتغالية", NameEN = "Portuguese" },
                new Language { Code = "tr", NameAR = "التركية",    NameEN = "Turkish" },
                new Language { Code = "fa", NameAR = "الفارسية",   NameEN = "Persian (Farsi)" },
                new Language { Code = "ur", NameAR = "الأردية",     NameEN = "Urdu" },
                new Language { Code = "hi", NameAR = "الهندية",    NameEN = "Hindi" },
                new Language { Code = "bn", NameAR = "البنغالية",  NameEN = "Bengali" },
                new Language { Code = "ms", NameAR = "الماليزية",  NameEN = "Malay" },
                new Language { Code = "id", NameAR = "الإندونيسية", NameEN = "Indonesian" },
                new Language { Code = "th", NameAR = "التايلاندية", NameEN = "Thai" },
                new Language { Code = "vi", NameAR = "الفيتنامية", NameEN = "Vietnamese" },
                new Language { Code = "sv", NameAR = "السويدية",   NameEN = "Swedish" },
                new Language { Code = "no", NameAR = "النرويجية",  NameEN = "Norwegian" },
                new Language { Code = "da", NameAR = "الدنماركية", NameEN = "Danish" },
                new Language { Code = "fi", NameAR = "الفنلندية",  NameEN = "Finnish" },
                new Language { Code = "nl", NameAR = "الهولندية",  NameEN = "Dutch" },
                new Language { Code = "el", NameAR = "اليونانية",  NameEN = "Greek" },
                new Language { Code = "he", NameAR = "العبرية",    NameEN = "Hebrew" },
                new Language { Code = "cs", NameAR = "التشيكية",   NameEN = "Czech" },
                new Language { Code = "pl", NameAR = "البولندية",  NameEN = "Polish" },
                new Language { Code = "ro", NameAR = "الرومانية",  NameEN = "Romanian" },
                new Language { Code = "hu", NameAR = "الهنغارية",  NameEN = "Hungarian" }
        };
         
        await context.Languages.AddRangeAsync(languages);
        await context.SaveChangesAsync();
    }
}