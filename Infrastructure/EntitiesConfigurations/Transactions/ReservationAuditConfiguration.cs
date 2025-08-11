using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfigurations;

public class ReservationAuditConfiguration : IEntityTypeConfiguration<ReservationAudit>
{
    public void Configure(EntityTypeBuilder<ReservationAudit> builder)
    {
        builder.ToTable("ReservationAudits");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName(name: "AuditID")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ReservationID)
            .IsRequired();

        builder.Property(x => x.Action)
            .HasConversion<byte>()
            .IsRequired();

        builder.Property(x => x.BorrowingID)
            .IsRequired(false);

        builder.Property(x => x.UserID)
            .IsRequired(false);

        builder.Property(x => x.Timestamp)
            .HasColumnName("Timestamp")
            .HasColumnType("datetime")
            .IsRequired();

        builder.HasOne(x => x.Reservation)
            .WithMany()
            .HasForeignKey(x => x.ReservationID)
            .OnDelete(DeleteBehavior.Restrict);

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