using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectricVehicleManagement.Data.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ElectricVehicleManagement.Data.Implementation.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.Phone).HasMaxLength(20);
            builder.Property(u => u.Address).HasMaxLength(500);
            builder.Property(u => u.Role).HasConversion<string>()
                .IsRequired();
            builder.Property(u => u.Status).HasDefaultValue(true);
            builder.Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(200);
            builder.HasIndex(u => u.Email)
                .IsUnique();
            builder.HasQueryFilter(u => u.Status);
        }
    }
}
