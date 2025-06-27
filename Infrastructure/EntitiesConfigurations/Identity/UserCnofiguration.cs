using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntitiesConfigurations;

public class UserCnofiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("UserID");

        //builder.ToTable(t => t.HasCheckConstraint("chk_SaudiPhoneNumber", "[PhoneNumber] LIKE '+966 5[0345689] ___ ____'"));

        builder.Property(x => x.PhoneNumber).HasMaxLength(20);


        builder.Property(x => x.RoleID).HasColumnName("RoleID").IsRequired();

        builder.HasAlternateKey(x => x.UserName).HasName("UQ_UserName");

        builder.HasOne(x => x.Role).WithMany(x => x.Users).HasForeignKey(x => x.RoleID);


    }
}

