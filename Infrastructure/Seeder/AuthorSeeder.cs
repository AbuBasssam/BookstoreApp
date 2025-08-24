using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seeder;

public static class AuthorSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Authors.AnyAsync())
            return;
        var authors = new List<Author>
        {
            new Author { NameEN = "J.K. Rowling", NameAR = "جيه كيه رولينج", BirthDate = new DateTime(1965, 7, 31), Bio = "British author, best known for the Harry Potter series." },
            new Author { NameEN = "George R.R. Martin", NameAR = "جورج ر. ر. مارتن", BirthDate = new DateTime(1948, 9, 20), Bio = "American novelist and short story writer, known for A Song of Ice and Fire." },
            new Author { NameEN = "Agatha Christie", NameAR = "أجاثا كريستي", BirthDate = new DateTime(1890, 9, 15), Bio = "English writer known for her detective novels and short stories." },
            new Author { NameEN = "Isaac Asimov", NameAR = "إسحاق أسيموف", BirthDate = new DateTime(1920, 1, 2), Bio = "American author and professor of biochemistry, known for his works of science fiction." },
            new Author { NameEN = "Yuval Noah Harari", NameAR = "يوفال نوح هراري", BirthDate = new DateTime(1976, 2, 24), Bio = "Israeli public intellectual, historian and professor in the Department of History at the Hebrew University of Jerusalem." }
        };
        await context.Authors.AddRangeAsync(authors);
        await context.SaveChangesAsync();
    }

}
