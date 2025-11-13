using ElectricVehicleManagement.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectricVehicleManagement.Data.Implementation.Configuration;

public class ListingImageConfiguration : IEntityTypeConfiguration<ListingImage>
{
    public void Configure(EntityTypeBuilder<ListingImage> builder)
    {
        builder.ToTable("ListingImages");

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

        builder.HasIndex(li => li.ListingId);
        builder.HasIndex(li => li.IsPrimary);
        builder.HasOne(li => li.Listing)
            .WithMany(l => l.Images)
            .HasForeignKey(li => li.ListingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}