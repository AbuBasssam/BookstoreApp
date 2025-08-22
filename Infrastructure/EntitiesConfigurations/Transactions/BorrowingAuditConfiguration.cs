using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfigurations;

public class BorrowingAuditConfiguration : IEntityTypeConfiguration<BorrowingAudit>
{
    public void Configure(EntityTypeBuilder<BorrowingAudit> builder)
    {
        builder.ToTable("BorrowingAudits");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("AuditID")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.BorrowingID)
            .HasColumnName("BorrowingID")
            .IsRequired();

        builder.Property(x => x.Action)
            .HasColumnName("Action")
            .HasConversion<byte>()
            .HasComment("1: Borrow Created, 2: Borrow Extended, 3: Borrow Returned")
            .IsRequired();

        builder.Property(x => x.UserID)
            .IsRequired();

        builder.Property(x => x.Timestamp)
            .HasColumnType("datetime")
            .IsRequired();

        builder.Property(x => x.OldDueDate)
            .HasColumnType("datetime")
            .IsRequired(false);

        builder.Property(x => x.NewDueDate)
            .HasColumnType("datetime")
            .IsRequired(false);

        //relationships configuration
        builder.HasOne(x => x.Borrowing)
            .WithMany()
            .HasForeignKey(x => x.BorrowingID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserID)
            .OnDelete(DeleteBehavior.Restrict);


    }
}
