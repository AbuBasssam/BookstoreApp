using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfigurations;

public class ReservationNotificationConfiguration : IEntityTypeConfiguration<ReservationNotification>
{
    public void Configure(EntityTypeBuilder<ReservationNotification> builder)
    {
        builder.ToTable("ReservationNotification");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("ID")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.NotificationID)
            .IsRequired();

        builder.Property(x => x.ReservationID)
            .IsRequired();

        builder.HasOne(x => x.Notification)
            .WithMany()
            .HasForeignKey(x => x.NotificationID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Reservation)
            .WithMany()
            .HasForeignKey(x => x.ReservationID)
            .OnDelete(DeleteBehavior.Cascade);

    }
}