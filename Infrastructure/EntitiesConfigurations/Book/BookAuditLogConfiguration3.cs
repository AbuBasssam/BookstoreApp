using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntitiesConfigurations;

public class BookAuditLogConfiguration : IEntityTypeConfiguration<BookAuditLog>
{
    public void Configure(EntityTypeBuilder<BookAuditLog> builder)
    {
        // Table configuration
        builder.ToTable("BookAuditLogs");

        // Primary key
        builder.HasKey(x => x.Id);

        // Properties configuration
        builder.Property(x => x.Id)
            .HasColumnName("ID")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.BookID)
            .HasColumnName("BookID")
            .IsRequired();

        builder.Property(x => x.CopyID)
            .HasColumnName("CopyID")
            .IsRequired(false);

        builder.Property(x => x.UpdatedFieldName)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(x => x.OldValue)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.NewValue)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.ActionType)
            .HasConversion<byte>()
            .IsRequired();

        builder.Property(x => x.ActionDate)
            .HasColumnType("datetime")
            .IsRequired();

        builder.Property(x => x.ByUserID)
            .IsRequired();

        // Foreign key relationships
        builder.HasOne(x => x.Book)
            .WithMany()
            .HasForeignKey(x => x.BookID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.ByUserID)
            .OnDelete(DeleteBehavior.Restrict);



        builder.HasIndex(x => x.ByUserID)
            .HasDatabaseName("IX_BookAuditLog_ByUserID");


    }
}
