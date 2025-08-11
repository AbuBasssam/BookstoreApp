using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfigurations;
internal class SystemSettingsConfiguration : IEntityTypeConfiguration<SystemSettings>
{
    public void Configure(EntityTypeBuilder<SystemSettings> builder)
    {
        builder.ToTable("SystemSettings");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
              .HasColumnName("SettingID")
              .ValueGeneratedOnAdd();

        builder.Property(s => s.FinePerDay).HasColumnType("decimal(5,2)");

        builder.Property(s => s.LastUpdated).HasDefaultValueSql("GETUTCDATE()");
    }
}
