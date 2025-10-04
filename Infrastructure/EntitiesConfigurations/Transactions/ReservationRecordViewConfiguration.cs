using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfigurations;

public class ReservationRecordViewConfiguration : IEntityTypeConfiguration<ReservationRecordView>
{
    public void Configure(EntityTypeBuilder<ReservationRecordView> builder)
    {
        builder.HasNoKey();

        builder.ToView("vw_ReservationRecords");

        builder.Property(e => e.Id).HasColumnName("ReservationRecordID");
        builder.Property(e => e.BookID).HasColumnName("BookID");
        builder.Property(e => e.BookTitleEN).HasColumnName("BookTitleEN");
        builder.Property(e => e.BookTitleAR).HasColumnName("BookTitleAR");
        builder.Property(e => e.BookISBN).HasColumnName("BookISBN");
        builder.Property(e => e.BookCoverImage).HasColumnName("BookCoverImage");
        builder.Property(e => e.MemberID).HasColumnName("MemberID");
        builder.Property(e => e.UserName).HasColumnName("UserName");
        builder.Property(e => e.MemberEmail).HasColumnName("MemberEmail");
        builder.Property(e => e.ReservationDate).HasColumnName("ReservationDate");
        builder.Property(e => e.ExpirationDate).HasColumnName("ExpirationDate");
        builder.Property(e => e.RemainingPickupHours).HasColumnName("RemainingPickupHours");
        builder.Property(e => e.WaitingQueuePosition).HasColumnName("WaitingQueuePosition");
        builder.Property(e => e.ReservationType).HasConversion<string>().HasColumnName("ReservationType");
        builder.Property(e => e.ReservationStatus).HasConversion<string>().HasColumnName("ReservationStatus");
        builder.Property(e => e.ReservationState).HasConversion<string>().HasColumnName("ReservationState");

    }
}
