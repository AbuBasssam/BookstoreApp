using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntitiesConfigurations;
public class BookCnofiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("Books");

        builder.ToTable(b => b.HasCheckConstraint(
            name: "CK_Books_Position_Format",
            sql: "Position LIKE '[A-Z][0-9][0-9]%' OR Position LIKE '[A-Z][0-9][0-9]-[0-9A-Za-z]%'"
        ));

        builder.ToTable("Books", b =>
        {
            b.HasTrigger("TR_Book_Update_Audit");
        });


        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id).HasColumnName("BookID").ValueGeneratedOnAdd();

        builder.Property(b => b.TitleEN).HasColumnType("nvarchar").IsRequired().HasMaxLength(50);

        builder.Property(b => b.TitleAR).HasColumnType("nvarchar").IsRequired().HasMaxLength(50);

        builder.Property(b => b.PublishDate).IsRequired();

        builder.Property(b => b.ISBN).HasColumnType("nvarchar").IsRequired().HasMaxLength(20);

        builder.Property(b => b.CoverImage)
            .HasColumnType("nvarchar")
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(b => b.DescriptionEN)
            .HasColumnType("nvarchar(300)");

        builder.Property(b => b.DescriptionAR)
            .HasColumnType("nvarchar(300)");

        builder.Property(b => b.PageCount)
            .HasColumnType("smallint");

        builder.Property(b => b.AvailabilityDate)
            .HasColumnType("date");

        builder.Property(b => b.Position)
            .HasColumnType("nvarchar(20)")
            .HasMaxLength(20)
            .HasComment("Format: [A-Z][2 digits] or [A-Z][2 digits]-[alphanumeric]");

        builder.Property(b => b.LastReservationOpenDate)
            .HasColumnType("datetime2(7)")
            .IsRequired(false);

        // relationships Configuration
        builder.HasOne(b => b.Category)
               .WithMany(c => c.Books)
               .HasForeignKey(b => b.CategoryID);

        builder.HasOne(b => b.Author)
               .WithMany(a => a.Books)
               .HasForeignKey(b => b.AuthorID);

        builder.HasOne(b => b.Publisher)
              .WithMany()
              .HasForeignKey(b => b.PublisherID)
              .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Language)
               .WithMany()
               .HasForeignKey(b => b.LanguageID)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

