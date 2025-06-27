using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntitiesConfigurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("CategoryID");
        builder.Property(c => c.NameEN).HasColumnType("nvarchar").IsRequired().HasMaxLength(50);
        builder.Property(c => c.NameAR).HasColumnType("nvarchar").IsRequired().HasMaxLength(50);
    }
}


