namespace ElectricVehicleManagement.Data.Models;

public class ListingImage
{
    public Guid Id { get; set; }
    public Guid ListingId { get; set; }
    public string ImageUrl { get; set; } = null!;
    public bool IsPrimary { get; set; } = false;
    public DateTime UploadedAt { get; set; }

    // Navigation property
    public Listing Listing { get; set; } = null!;
}