using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfigurations;

public class BorrowingRecordViewConfiguration : IEntityTypeConfiguration<BorrowingRecordView>
{
    public void Configure(EntityTypeBuilder<BorrowingRecordView> builder)
    {
        builder.HasNoKey();
        builder.ToView("vw_BorrowingRecord");

        builder.Property(b => b.Id).HasColumnName("BorrowingRecordId");
        builder.Property(b => b.BookCopyID).HasColumnName("BookCopyID");
        builder.Property(b => b.BookId).HasColumnName("BookId");
        builder.Property(b => b.MemberID).HasColumnName("MemberID");
        builder.Property(b => b.MemberName).HasColumnName("MemberName");
        builder.Property(b => b.ReservationRecordID).HasColumnName("ReservationRecordID");
        builder.Property(b => b.ReservationDate).HasColumnName("ReservationDate");
        builder.Property(b => b.BorrowingDate).HasColumnName("BorrowingDate");
        builder.Property(b => b.DueDate).HasColumnName("DueDate");
        builder.Property(b => b.ReturnDate).HasColumnName("ReturnDate");
        builder.Property(b => b.RenewalCount).HasColumnName("RenewalCount");
        builder.Property(b => b.AdminID).HasColumnName("AdminID");
        builder.Property(b => b.AdminName).HasColumnName("AdminName");
        builder.Property(b => b.TotalFines).HasColumnName("TotalFines");
        builder.Property(b => b.BorrowingStatus).HasConversion<string>().HasColumnName("BorrowingStatus");
    }
}