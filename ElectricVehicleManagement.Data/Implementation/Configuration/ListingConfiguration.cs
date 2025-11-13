using ElectricVehicleManagement.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectricVehicleManagement.Data.Implementation.Configuration;

public class ListingConfiguration : IEntityTypeConfiguration<Listing>
{
    public void Configure(EntityTypeBuilder<Listing> builder)
    {
        builder.ToTable("Listings");
        builder.HasKey(l => l.ListingId);
        
        builder.HasOne(l => l.User).WithMany(l => l.Listings).HasForeignKey(l => l.UserId);
        
        builder.Property(l => l.Title)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(l => l.Description);

        builder.Property(l => l.Price)
            .IsRequired()
            .HasColumnType("decimal(20,2)");

        builder.Property(l => l.BatteryCapacityKwh);
        builder.Property(l => l.BatteryConditionPercent)
            .HasColumnType("decimal(5,2)");

        builder.Property(l => l.MileageKm);
        builder.Property(l => l.ManufacturingYear)
            .IsRequired();

        builder.Property(l => l.VehicleBrand)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(l => l.VehicleModel)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(l => l.Location)
            .HasMaxLength(255);
        builder.Property(l => l.SeatingCapacity);
        builder.Property(v => v.CreatedAt)
            .HasColumnType("timestamptz")
            .IsRequired();
        builder.Property(v => v.UpdatedAt)
            .HasColumnType("timestamptz")
            .IsRequired();
        
        
        builder.Property(v => v.Status).HasConversion<byte>();
        builder.Property(v => v.BodyType).HasConversion<byte>();
        builder.Property(v => v.TransmissionType).HasConversion<byte>();
        builder.Property(v => v.Energy).HasConversion<byte>();
    }
}