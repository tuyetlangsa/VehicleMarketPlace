using ElectricVehicleManagement.Data.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace ElectricVehicleManagement.Service.Listing;

public class ListingService(IDbContext dbContext) : IListingService
{
    public async Task CreateListing(Data.Models.Listing listing)
    {
        dbContext.Listings.Add(listing);
        await dbContext.SaveChangesAsync();
    }

    public async Task<List<Data.Models.Listing>> GetListings()
    {
        return await dbContext.Listings
            .Include(l => l.Images)
            .ToListAsync();
    }

    
}