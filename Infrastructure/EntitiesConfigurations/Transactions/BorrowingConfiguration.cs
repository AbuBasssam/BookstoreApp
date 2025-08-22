using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfigurations;
internal class BorrowingConfiguration : IEntityTypeConfiguration<BorrowingRecord>
{
    public void Configure(EntityTypeBuilder<BorrowingRecord> builder)
    {
        builder.ToTable("BorrowingRecords");
        builder.ToTable(x =>
        {
            x.HasTrigger("TR_BorrowingRecord_Update_Audit");
            x.HasTrigger("TR_BorrowingRecord_Insert_Audit");
            x.HasTrigger("TR_BorrowingRecord_Insert");
        });

        builder.HasKey(br => br.Id);

        builder.Property(br => br.Id).HasColumnName("BorrowingRecordID");

        builder.Property(br => br.BorrowingDate)
               .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(br => br.RenewalCount)
            .HasColumnType("tinyint")
            .HasDefaultValue(0);

        builder.HasOne(br => br.BookCopy)
               .WithMany(b => b.BorrowingRecords)
               .HasForeignKey(l => l.BookCopyID)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(br => br.Member)
               .WithMany()
               .HasForeignKey(l => l.MemberID)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.Admin)
               .WithMany()
               .HasForeignKey(l => l.AdminID)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

