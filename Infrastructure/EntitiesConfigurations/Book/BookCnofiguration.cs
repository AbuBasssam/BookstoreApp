using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntitiesConfigurations;
public class BookCnofiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("Books");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).HasColumnName("BookID").ValueGeneratedOnAdd();
        builder.Property(b => b.TitleEN).HasColumnType("nvarchar").IsRequired().HasMaxLength(50);
        builder.Property(b => b.TitleAR).HasColumnType("nvarchar").IsRequired().HasMaxLength(50);
        builder.Property(b => b.PublishDate).IsRequired();
        builder.Property(b => b.ISBN).HasColumnType("nvarchar").IsRequired().HasMaxLength(20);
        builder.Property(b => b.NumberOfCopies)
            .HasColumnType("tinyint")
            .HasComputedColumnSql("dbo.fn_GetBookCopyCount(BookID)");

        // relationships Configuration
        builder.HasOne(b => b.Category)
               .WithMany(c => c.Books)
               .HasForeignKey(b => b.CategoryID);

        builder.HasOne(b => b.Author)
               .WithMany(a => a.Books)
               .HasForeignKey(b => b.AuthorID);
    }
}


