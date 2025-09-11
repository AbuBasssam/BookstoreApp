using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seeder;

public class BorrowingRecordSeeder
{
    private static readonly Random _rand = new Random();

    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.BorrowingRecords.AnyAsync())
            return;
        await context.Database.ExecuteSqlRawAsync("DISABLE TRIGGER ALL ON BorrowingRecords");


        var admins = await context.Users.Where(u => u.RoleID == 1).Select(u => u.Id).ToListAsync();
        var members = await context.Users.Where(u => u.RoleID == 2).Select(u => u.Id).ToListAsync();
        var BookCopiesIds = await context.BookCopies.Select(u => u.Id).ToListAsync();

        if (!admins.Any() || !members.Any())
            return;

        int adminId = admins.First();
        var records = new List<BorrowingRecord>();

        // كل 5 نسخ تخص كتاب واحد
        int totalCopies = context.BookCopies.Count();
        DateTime startDate = DateTime.Today.AddMonths(-2);

        for (int i = 0; i < totalCopies; i++)
        {
            // توزيع غير متساوي للاستعارات
            int borrowCount = WeightedBorrowCount();

            for (int j = 0; j < borrowCount; j++)
            {
                var borrowDate = startDate.AddDays(_rand.Next(0, 60));
                var dueDate = borrowDate.AddDays(14);

                records.Add(new BorrowingRecord
                {
                    BookCopyID = BookCopiesIds[i],
                    MemberID = members[_rand.Next(members.Count)],
                    ReservationRecordID = null,
                    BorrowingDate = borrowDate,
                    DueDate = dueDate,
                    ReturnDate = (j == borrowCount - 1 && _rand.Next(0, 2) == 0)
                                    ? null // استعارة حالية لم تُرجع بعد
                                    : borrowDate.AddDays(_rand.Next(1, 20)),
                    RenewalCount = 0,
                    AdminID = adminId
                });
            }
        }

        await context.BorrowingRecords.AddRangeAsync(records);
        await context.SaveChangesAsync();
    }

    private static int WeightedBorrowCount()
    {
        // توزيع احتمالي للاستعارات لكل نسخة
        int roll = _rand.Next(100);
        if (roll < 40) return 0;   // 40% من النسخ بلا استعارة
        if (roll < 70) return 1;   // 30% استعارة واحدة
        if (roll < 90) return 2;   // 20% استعارتين
        if (roll < 98) return 3;   // 8% ثلاث استعارات
        return 4;                  // 2% أربع استعارات (كتاب شائع جدًا)
    }
}
