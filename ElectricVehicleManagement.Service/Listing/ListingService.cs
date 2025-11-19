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

    public async Task<Data.Models.Listing?> GetListingById(Guid id)
    {
        return await dbContext.Listings.FindAsync(id);
    }

    public async Task<List<Data.Models.Listing>> SearchListings(string query)
    {
        return await dbContext.Listings
            .Where(l =>
                EF.Functions.ToTsVector("english",
                        l.Title + " " +
                        l.Description + " " +
                        l.VehicleBrand + " " +
                        l.VehicleModel + " " +
                        l.Location + " " +
                        l.SeatingCapacity + " " +
                        l.Energy + " " +
                        l.BodyType + " " +
                        l.ManufacturingYear + " " +
                        l.TransmissionType
                    )
                    .Matches(query)
            )
            .ToListAsync();
    }

    public async Task<List<Data.Models.Listing>> GetListings()
    {
        return await dbContext.Listings
            .Include(l => l.Images)
            .ToListAsync();
    }

    
}