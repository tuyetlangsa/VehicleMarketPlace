using ElectricVehicleManagement.Data.Models.Enums;

namespace ElectricVehicleManagement.Data.Models;

public class Listing
{
    public Guid ListingId { get; set; }
    public Guid UserId { get; set; }
    public Guid? VehicleId { get; set; } // optional
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int? BatteryCapacityKwh { get; set; }
    public decimal? BatteryConditionPercent { get; set; }
    public int? MileageKm { get; set; }
    public int ManufacturingYear { get; set; }
    public string VehicleBrand { get; set; } = null!;
    public string VehicleModel { get; set; } = null!;
    public TransmissionType TransmissionType { get; set; } // automatic, manual
    public int SeatingCapacity { get; set; }
    public BodyType BodyType { get; set; } // sedan, suv, hatchback, etc
    public Energy Energy { get; set; }
    public string? Location { get; set; }
    public ListingStatus Status { get; set; } // pending, approved, reject, sold. 
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public Vehicle? Vehicle { get; set; }
    public ICollection<ListingImage> Images { get; set; } = new List<ListingImage>();
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
}