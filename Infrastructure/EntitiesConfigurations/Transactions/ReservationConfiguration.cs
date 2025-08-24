using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfigurations;

internal class ReservationConfiguration : IEntityTypeConfiguration<ReservationRecord>
{
    public void Configure(EntityTypeBuilder<ReservationRecord> builder)
    {
        builder.ToTable("ReservationRecords");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("ReservationRecordID");

        builder.Property(r => r.ReservationDate)
              .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(r => r.Type)
               .HasConversion<byte>()
               .IsRequired();

        builder.Property(r => r.Status)
               .HasConversion<byte>()
               .HasComment("1: Pending, 2: Notified, 3: Fulfilled, 4: Expired, 5:Cancelled")
               .IsRequired();

        builder.Property(r => r.ExpirationDate)
            .IsRequired(false);

        // Relationships configuration
        builder.HasOne(r => r.Book)
               .WithMany(b => b.Reservations)
               .HasForeignKey(r => r.BookID)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Member)
               .WithMany()
               .HasForeignKey(r => r.MemberID)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

