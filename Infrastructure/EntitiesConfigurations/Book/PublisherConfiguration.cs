using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntitiesConfigurations;

internal class PublisherConfiguration : IEntityTypeConfiguration<Publisher>
{
    public void Configure(EntityTypeBuilder<Publisher> builder)
    {
        // Table
        builder.ToTable("Publishers");

        // Primary Key
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasColumnName("PublisherID")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.NameEN)
            .HasColumnType("NVARCHAR(50)")
            .IsRequired();

        builder.Property(p => p.NameAR)
            .HasColumnType("NVARCHAR(50)")
            .IsRequired();


    }
}