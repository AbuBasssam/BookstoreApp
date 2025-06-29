using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfigurations;

internal class UserDeviceConfiguration : IEntityTypeConfiguration<UserDevice>
{
    public void Configure(EntityTypeBuilder<UserDevice> builder)
    {
        builder.ToTable("UserDevices");

        // Primary Key
        builder.HasKey(d => d.Id);
        
        builder.Property(d => d.Id)
            .HasColumnName("DeviceID")
            .ValueGeneratedOnAdd();

        builder.Property(d => d.DeviceName)
            .HasMaxLength(50);

        builder.Property(d => d.Platform)
            .HasConversion<byte>()
            .IsRequired();

        builder.Property(d => d.LastActive)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(d => d.IsActive)
            .HasDefaultValue(true);

        // Relationships
        builder.HasOne(d => d.User)
            .WithMany(u => u.Devices)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}