using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntitiesConfigurations;

public class BookViewConfiguration : IEntityTypeConfiguration<BookView>
{
    public void Configure(EntityTypeBuilder<BookView> builder)
    {
        builder.HasNoKey();
        builder.ToView("BookView");

        builder.Property(b => b.Id).HasColumnName("BookId");

        builder.Property(b => b.TitleEN).HasColumnType("nvarchar").IsRequired().HasMaxLength(50);

        builder.Property(b => b.ISBN).HasColumnType("nvarchar").IsRequired().HasMaxLength(20);

        builder.Property(b => b.ImageUrl).HasColumnType("nvarchar")
           .IsRequired()
           .HasMaxLength(250);


        builder.Property(b => b.TitleAR).HasColumnType("nvarchar").IsRequired().HasMaxLength(50);
        //builder.Property(b => b.IsAvailable).HasComputedColumnSql(
        //"(CASE WHEN EXISTS (SELECT 1 FROM BookCopies WHERE BookId = Id AND IsAvailable = 1) THEN 1 ELSE 0 END)");
    }
}

