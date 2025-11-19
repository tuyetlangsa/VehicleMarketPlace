using ElectricVehicleManagement.Data.Models;
using ElectricVehicleManagement.Data.Models.Dto;

namespace ElectricVehicleManagement.Service.Listing;

public interface IListingService
{
    public Task<List<Data.Models.Listing>> GetListings();

    public Task CreateListing(Data.Models.Listing listing);
    
    public Task<Data.Models.Listing?> GetListingById(Guid id);

    public Task<List<Data.Models.Listing>> SearchListings(string query);

    public Task<PagedResult<Data.Models.Listing>> GetListingsByUser(Guid userId, int page, int pageSize, string keyword);

    public Task DeleteListing(Guid listingId);


    public Task UpdateListing(Data.Models.Listing updated,
        List<Guid> remainingImageIds,
        List<string> newUrls);
    
    public Task<Data.Models.Listing> GetListingById(Guid id);
    
    public Task UpdateVisibility(Guid listingId, bool isVisible);


}