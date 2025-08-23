using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntitiesConfigurations;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.HasKey(a => a.Id);
        builder.ToTable("Authors");
        builder.Property(a => a.Id).HasColumnName("AuthorID").ValueGeneratedOnAdd();
        builder.Property(a => a.NameEN).HasColumnName("AuthorNameEN").HasColumnType("nvarchar").IsRequired().HasMaxLength(50);
        builder.Property(a => a.NameAR).HasColumnName("AuthorNameAR").HasColumnType("nvarchar").IsRequired().HasMaxLength(50);
        builder.Property(a => a.Bio).HasMaxLength(200);
    }
}


