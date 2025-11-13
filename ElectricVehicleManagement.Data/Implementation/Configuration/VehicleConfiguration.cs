using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectricVehicleManagement.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectricVehicleManagement.Data.Implementation.Configuration
{

    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.ToTable("Vehicles");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.Brand)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(v => v.Model)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(v => v.Year)
                .IsRequired();

            builder.Property(v => v.Color)
                .HasMaxLength(50);

            builder.Property(v => v.CreatedAt)
                .HasColumnType("timestamptz")
                .IsRequired();

            builder.Property(v => v.UpdatedAt)
                .HasColumnType("timestamptz")
                .IsRequired();
            builder.Property(v => v.BodyType).HasConversion<byte>();
            builder.Property(v => v.TransmissionType).HasConversion<byte>();
            builder.Property(v => v.Energy).HasConversion<byte>();

            builder.HasIndex(v => v.UserId);
            builder.HasIndex(v => v.Brand);
            builder.HasIndex(v => v.Model);
            builder.HasIndex(v => v.Year);
            
            // Relationship: Vehicle belongs to a User
            builder.HasOne(v => v.User)
                .WithMany(u => u.Vehicles)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
    
}
