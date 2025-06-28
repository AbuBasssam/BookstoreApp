using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfigurations;

internal class ReservationConfiguration : IEntityTypeConfiguration<ReservationRecord>
{
    public void Configure(EntityTypeBuilder<ReservationRecord> builder)
    {
        builder.ToTable("ReservationRecords");
        
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.Id)
            .HasColumnName("ReservationRecordID");

        builder.Property(r => r.ReservationDate)
              .HasDefaultValueSql("GETUTCDATE()");


        builder.Property(r => r.IsCancelled)
               .HasDefaultValue(false);

       
        builder.HasOne(r => r.Book)
               .WithMany(b => b.Reservations)
               .HasForeignKey(r => r.BookID)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Member)
               .WithMany()
               .HasForeignKey(r => r.MemberID)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

