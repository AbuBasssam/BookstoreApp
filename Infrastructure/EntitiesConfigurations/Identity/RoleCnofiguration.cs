using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntitiesConfigurations;
public class RoleCnofiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");


        builder.HasKey(x => x.Id);


        builder.Property(x => x.Id).HasColumnName("RoleID").ValueGeneratedOnAdd();


        builder.Property(r => r.Name).HasMaxLength(50);


        builder.Property(r => r.NormalizedName).HasMaxLength(50);


        builder.Property(r => r.ConcurrencyStamp).HasMaxLength(64);


    }
}

