using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntitiesConfigurations;
internal class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.ToTable("Languages");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id)
            .HasColumnName("LanguageID")
            .ValueGeneratedOnAdd();

        builder.Property(l => l.Code)
            .HasColumnType("char(2)")
            .IsRequired();

        builder.Property(l => l.NameEN)
            .HasColumnName("LanguageNameEN")
            .HasColumnType("NVARCHAR(50)")
            .IsRequired();


        builder.Property(l => l.NameAR)
            .HasColumnName("LanguageNameAR")
            .HasColumnType("NVARCHAR(50)")
            .IsRequired();

        // Unique Constraint on Code
        builder.HasIndex(l => l.Code)
            .IsUnique()
            .HasDatabaseName("UQ_Languages_Code");

    }
}
