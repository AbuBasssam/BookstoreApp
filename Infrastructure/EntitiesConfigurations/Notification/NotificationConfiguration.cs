using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfigurations;
internal class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");

        // Primary Key
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Id)
            .HasColumnName("NotificationID")
            .ValueGeneratedOnAdd();

        // Properties Configuration
        builder.Property(n => n.Title)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(n => n.Message)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(n => n.SentDate)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(n => n.IsRead)
            .HasDefaultValue(false);

        builder.Property(n => n.NotificationType)
            .HasConversion<byte>()
            .IsRequired();

        builder.Property(n => n.EntityType)
            .HasConversion<byte>();

        // Relationships
        builder.HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(n => n.UserDevice)
            .WithMany()
            .HasForeignKey(n => n.UserDeviceId)
            .OnDelete(DeleteBehavior.Restrict);


    }


}
