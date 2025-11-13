using ElectricVehicleManagement.Data.Models.Enums;

namespace ElectricVehicleManagement.Data.Models;

public class Vehicle
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Brand { get; set; } = null!;
    public string Model { get; set; } = null!;
    public int Year { get; set; }
    public string? Color { get; set; }
    public Energy Energy { get; set; }
    public BodyType BodyType { get; set; }
    public TransmissionType TransmissionType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public User User { get; set; } = null!;
    public ICollection<Listing> Listings { get; set; } = new List<Listing>();
    public ICollection<VehicleImage> Images { get; set; } = new List<VehicleImage>();

}