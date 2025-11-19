namespace ElectricVehicleManagement.Service.Listing;

public interface IListingService
{
    public Task<List<Data.Models.Listing>> GetListings();

    public Task CreateListing(Data.Models.Listing listing);
    
    public Task<Data.Models.Listing?> GetListingById(Guid id);

    public Task<List<Data.Models.Listing>> SearchListings(string query);

}