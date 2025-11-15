using ElectricVehicleManagement.Data.Models.Enums;

namespace ElectricVehicleManagement.Data.Models;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string FullName { get; set; } = null!;
    public string? Address { get; set; }
    public Role Role { get; set; } 
    public bool Status { get; set; }
    
    public ICollection<Listing> Listings { get; set; } = new List<Listing>();
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

}