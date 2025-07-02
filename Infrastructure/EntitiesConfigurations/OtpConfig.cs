using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntitiesConfigurations
{
    public class OtpConfig : IEntityTypeConfiguration<Otp>
    {
        public void Configure(EntityTypeBuilder<Otp> builder)
        {
            builder.ToTable("Otps");

            builder.ToTable(t => t.HasCheckConstraint("CK_Otp_Type", "Type > 0 AND Type < 3"));

            builder.HasKey(o => o.Id);

            // Configure properties

            builder.Property(o => o.Code)
                .HasColumnType("char")
                .HasMaxLength(44);

            builder.Property(o => o.CreationTime);

            builder.Property(o => o.ExpirationTime);

            builder.Property(c => c.Type)
            .HasConversion<byte>()
            .IsRequired();

            // Configure the foreign key relationship
            builder.HasOne(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserID)
            .OnDelete(DeleteBehavior.Cascade);



        }
    }


}
