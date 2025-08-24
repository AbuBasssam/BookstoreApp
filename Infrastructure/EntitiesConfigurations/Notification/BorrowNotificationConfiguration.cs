using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfigurations;

public class BorrowNotificationConfiguration : IEntityTypeConfiguration<BorrowNotification>
{
    public void Configure(EntityTypeBuilder<BorrowNotification> builder)
    {
        builder.ToTable("BorrowNotifications");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("ID")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.NotificationID)
            .IsRequired();

        builder.Property(x => x.BorrowID)
            .IsRequired();

        builder.HasOne(x => x.Notification)
            .WithMany()
            .HasForeignKey(x => x.NotificationID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Borrow)
            .WithMany()
            .HasForeignKey(x => x.BorrowID)
            .OnDelete(DeleteBehavior.Cascade);

    }
}