using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntitiesConfigurations;

public class BookCopyConfiguration : IEntityTypeConfiguration<BookCopy>
{
    public void Configure(EntityTypeBuilder<BookCopy> builder)
    {
        builder.ToTable("BookCopies");
        builder.HasKey(bc => bc.Id);
        builder.Property(bc => bc.Id).HasColumnName("CopyID").ValueGeneratedOnAdd();
        builder.Property(bc => bc.IsAvailable).IsRequired();

        // relationships Configuration
        builder.HasOne(bc => bc.Book).WithMany(b => b.Copies).HasForeignKey(bc => bc.BookID);//one-to-many relationship from BookCopy to Book
    }
}


