namespace ElectricVehicleManagement.Data.Models.Dto;

public class PagedResult<A>
{
    public List<A> Items { get; set; } = new();
    public int Page { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
    
}