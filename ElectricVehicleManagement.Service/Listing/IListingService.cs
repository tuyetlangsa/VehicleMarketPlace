namespace ElectricVehicleManagement.Service.Listing;

public interface IListingService
{
    public Task<List<Data.Models.Listing>> GetListings();    
}