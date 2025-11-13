namespace ElectricVehicleManagement.Data.Models;

public class Favorite
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ListingId { get; set; }
// Navigation properties
    public User User { get; set; } = null!;
    public Listing Listing { get; set; } = null!;
}