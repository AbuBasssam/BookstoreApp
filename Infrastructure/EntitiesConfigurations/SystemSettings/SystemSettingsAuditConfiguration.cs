using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfigurations;

public class SystemSettingsAuditConfiguration : IEntityTypeConfiguration<SystemSettingsAudit>
{
    public void Configure(EntityTypeBuilder<SystemSettingsAudit> builder)
    {
        builder.ToTable("SystemSettingsAudits");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("AuditID")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.SettingName)
            .HasColumnName("SettingName")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.OldValue)
            .HasColumnName("OldValue")
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.NewValue)
            .HasColumnName("NewValue")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.ChangedBy)
            .HasColumnName("ChangedBy")
            .IsRequired();

        builder.Property(x => x.ChangeDate)
            .HasColumnName("ChangeDate")
            .HasColumnType("datetime")
            .IsRequired();

       
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.ChangedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ChangedBy)
            .HasDatabaseName("IX_SystemSettingsAudits_ChangedBy");

    }
}