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
        builder.Property(n => n.TitleEN)
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(n => n.TitleAR)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(n => n.MessageEN)
            .HasMaxLength(500)
            .IsRequired();
        builder.Property(n => n.MessageAR)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(n => n.SentDate)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(n => n.IsRead)
            .HasDefaultValue(false);

        builder.Property(n => n.NotificationType)
            .HasConversion<byte>()
            .IsRequired();


        // Relationships

        builder.HasOne(n => n.UserDevice)
            .WithMany()
            .HasForeignKey(n => n.UserDeviceID)
            .OnDelete(DeleteBehavior.Restrict);


    }


}
