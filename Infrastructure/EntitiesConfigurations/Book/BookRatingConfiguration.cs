using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntitiesConfigurations;

public class BookRatingConfiguration : IEntityTypeConfiguration<BookRating>
{
    public void Configure(EntityTypeBuilder<BookRating> builder)
    {
        builder.ToTable("BookRatings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("ID")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.BookID)
            .IsRequired();

        builder.Property(x => x.UserID)
            .IsRequired();

        builder.Property(x => x.Rating)
            .HasColumnName("Rating")
            .IsRequired();

        builder.ToTable(br => br.HasCheckConstraint("CK_BookRating_Rating", "[Rating] >= 1 AND [Rating] <= 5"));

        // Foreign key relationships
        builder.HasOne(x => x.Book)
            .WithMany()
            .HasForeignKey(x => x.BookID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.BookID)
            .HasDatabaseName("IX_BookRating_BookID");
    }
}