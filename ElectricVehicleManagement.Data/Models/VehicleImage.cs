namespace ElectricVehicleManagement.Data.Models;

public class VehicleImage
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public string ImageUrl { get; set; } = null!;
    public bool IsPrimary { get; set; } = false;
    public DateTime UploadedAt { get; set; }

    // Navigation property
    public Vehicle Vehicle { get; set; } = null!;
}