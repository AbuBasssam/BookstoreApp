using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntitiesConfigurations;

public class BookViewConfiguration : IEntityTypeConfiguration<BookView>
{
    public void Configure(EntityTypeBuilder<BookView> builder)
    {
        builder.HasNoKey();
        builder.ToView("vw_Books");

        // Columns configuration
        builder.Property(b => b.Id).HasColumnName("BookID");
        builder.Property(b => b.ISBN).HasMaxLength(20);
        builder.Property(b => b.TitleEN).HasMaxLength(50);
        builder.Property(b => b.TitleAR).HasMaxLength(50);
        builder.Property(b => b.DescriptionEN).HasMaxLength(300);
        builder.Property(b => b.DescriptionAR).HasMaxLength(300);
        builder.Property(b => b.PublisherEN).HasMaxLength(50);
        builder.Property(b => b.PublisherAR).HasMaxLength(50);
        builder.Property(b => b.AuthorEN).HasMaxLength(50);
        builder.Property(b => b.AuthorAR).HasMaxLength(50);
        builder.Property(b => b.LanguageEN).HasMaxLength(50);
        builder.Property(b => b.LanguageAR).HasMaxLength(50);
        builder.Property(b => b.CategoryEN).HasMaxLength(50);
        builder.Property(b => b.CategoryAR).HasMaxLength(50);
        builder.Property(b => b.Position).HasMaxLength(20);
        builder.Property(b => b.CoverImage).HasMaxLength(250);

        // Computed columns
        builder.Property(b => b.IsBorrowable);
        builder.Property(b => b.IsReservable);
        builder.Property(b => b.Rating).HasColumnType("decimal(3,2)");

    }
}

