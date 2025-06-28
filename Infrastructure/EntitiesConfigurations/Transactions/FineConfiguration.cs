using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfigurations;

internal class FineConfiguration : IEntityTypeConfiguration<Fine>
{
    public void Configure(EntityTypeBuilder<Fine> builder)
    {
        builder.ToTable("Fines");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id)
               .HasColumnName("FineID");
        
        builder.Property(f => f.TotalLateDays)
            .HasColumnType("tinyint")
            .HasDefaultValue(1);

        builder.Property(f => f.Amount)
              .HasColumnType("decimal(5,2)");

        builder.Property(f => f.IssueDate)
               .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(f => f.IsPaid)
            .HasDefaultValue(false);

        builder.HasOne(f => f.Borrowing)
               .WithMany(l => l.Fines)
               .HasForeignKey(f => f.BorrowingID)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

