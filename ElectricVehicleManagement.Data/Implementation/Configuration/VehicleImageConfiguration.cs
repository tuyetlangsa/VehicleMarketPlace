using ElectricVehicleManagement.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectricVehicleManagement.Data.Implementation.Configuration;


    public class VehicleImageConfiguration : IEntityTypeConfiguration<VehicleImage>
    {
        public void Configure(EntityTypeBuilder<VehicleImage> builder)
        {
            builder.ToTable("VehicleImages");

            builder.HasKey(li => li.Id);

            builder.Property(li => li.ImageUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(li => li.IsPrimary)
                .IsRequired()
                .HasDefaultValue(false);



            builder.Property(li => li.UploadedAt)
                .HasColumnType("timestamptz")
                .IsRequired();

            builder.HasIndex(li => li.VehicleId);
            builder.HasIndex(li => li.IsPrimary);
            builder.HasOne(li => li.Vehicle)
                .WithMany(l => l.Images)
                .HasForeignKey(li => li.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
