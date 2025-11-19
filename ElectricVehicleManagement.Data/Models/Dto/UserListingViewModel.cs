using ElectricVehicleManagement.Data.Models.Enums;

namespace ElectricVehicleManagement.Data.Models.Dto;

public class UserListingViewModel
{
    public Guid ListingId { get; set; }
    public string Title { get; set; } = null!;
    public decimal Price { get; set; }
    public ListingStatus Status { get; set; }
    public bool IsVisible { get; set; }
    public string? PrimaryImageUrl { get; set; }
}