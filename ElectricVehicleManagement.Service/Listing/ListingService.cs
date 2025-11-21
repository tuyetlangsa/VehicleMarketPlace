using ElectricVehicleManagement.Data.Abstraction;
using ElectricVehicleManagement.Data.Models;
using ElectricVehicleManagement.Data.Models.Dto;
using ElectricVehicleManagement.Data.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace ElectricVehicleManagement.Service.Listing;

public class ListingService(IDbContext dbContext) : IListingService
{
    public async Task CreateListing(Data.Models.Listing listing)
    {
        dbContext.Listings.Add(listing);
        await dbContext.SaveChangesAsync();
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
            .Include(l => l.User)
            .Include(l => l.Images)
            .Where(l => l.Status == ListingStatus.Approved)
            .ToListAsync();
    }

    public async Task<PagedResult<Data.Models.Listing>> GetListingsByUser(
        Guid userId,
        int page,
        int pageSize,
        string keyword)
    {
       
        var query = dbContext.Listings
            .Include(l => l.Images)
            .Where(l => l.UserId == userId);

        
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            keyword = keyword.ToLower();
            query = query.Where(l =>
                l.Title.ToLower().Contains(keyword) ||
                l.VehicleBrand.ToLower().Contains(keyword) ||
                l.VehicleModel.ToLower().Contains(keyword) ||
                (l.Location != null && l.Location.ToLower().Contains(keyword))
            );
        }
        
        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        
        if (page < 1) page = 1;
        if (page > totalPages && totalPages > 0) page = totalPages;
        
        var items = await query
            .OrderByDescending(l => l.UpdatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Data.Models.Listing>
        {
            Items = items,
            Page = page,
            TotalPages = totalPages,
            TotalItems = totalItems
        };
    }



    public async Task DeleteListing(Guid listingId)
    {
        var listing = await dbContext.Listings
            .Include(l => l.Images)
            .FirstOrDefaultAsync(l => l.ListingId == listingId);

        if (listing == null) return;

        dbContext.ListingImages.RemoveRange(listing.Images);
        dbContext.Listings.Remove(listing);

        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateListing(Data.Models.Listing updated,
        List<Guid> remainingImageIds,
        List<string> newUrls)
    {

        var listing = await dbContext.Listings
            .FirstOrDefaultAsync(l => l.ListingId == updated.ListingId);

        if (listing == null)
            return;


        listing.Title = updated.Title;
        listing.Description = updated.Description;
        listing.Price = updated.Price;
        listing.ManufacturingYear = updated.ManufacturingYear;
        listing.VehicleBrand = updated.VehicleBrand;
        listing.VehicleModel = updated.VehicleModel;
        listing.TransmissionType = updated.TransmissionType;
        listing.SeatingCapacity = updated.SeatingCapacity;
        listing.BodyType = updated.BodyType;
        listing.Energy = updated.Energy;
        listing.MileageKm = updated.MileageKm;
        listing.BatteryCapacityKwh = updated.BatteryCapacityKwh;
        listing.BatteryConditionPercent = updated.BatteryConditionPercent;
        listing.Location = updated.Location;

        listing.UpdatedAt = DateTime.UtcNow;
        listing.Status = ListingStatus.Pending;

        
        var oldImages = await dbContext.ListingImages
            .Where(i => i.ListingId == listing.ListingId)
            .ToListAsync();

        dbContext.ListingImages.RemoveRange(oldImages);

        foreach (var id in remainingImageIds)
        {
            var old = oldImages.First(x => x.Id == id);

            dbContext.ListingImages.Add(new ListingImage
            {
                Id = id,
                ListingId = listing.ListingId,
                ImageUrl = old.ImageUrl,
                IsPrimary = old.IsPrimary,
                UploadedAt = old.UploadedAt
            });
        }
        
        foreach (var url in newUrls)
        {
            dbContext.ListingImages.Add(new ListingImage
            {
                Id = Guid.NewGuid(),
                ListingId = listing.ListingId,
                ImageUrl = url,
                IsPrimary = false,
                UploadedAt = DateTime.UtcNow
            });
        }
        
        var finalImages = await dbContext.ListingImages
            .Where(x => x.ListingId == listing.ListingId)
            .ToListAsync();

        if (!finalImages.Any(i => i.IsPrimary))
        {
            finalImages.First().IsPrimary = true;
        }

        await dbContext.SaveChangesAsync();
    }


    public async Task<Data.Models.Listing> GetListingById(Guid id)
    {
        return await dbContext.Listings
            .AsNoTracking()
            .Include(l => l.Images)
            .FirstOrDefaultAsync(l => l.ListingId == id);
    }
    
    public async Task UpdateVisibility(Guid listingId, bool isVisible)
    {
        var listing = await dbContext.Listings
            .FirstOrDefaultAsync(l => l.ListingId == listingId);

        if (listing == null) return;

        listing.IsVisible = isVisible;
        listing.UpdatedAt = DateTime.UtcNow;
        
        await dbContext.SaveChangesAsync();
    }

    public async Task<List<Data.Models.Listing>> GetPendingListings()
    {
        return await dbContext.Listings
            .AsNoTracking()
            .Include(l => l.Images)
            .Where(l => l.Status == ListingStatus.Pending)
            .OrderBy(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> ApproveListing(Guid listingId)
    {
        var listing = await dbContext.Listings.FirstOrDefaultAsync(l => l.ListingId == listingId);
        if (listing == null) return false;
        if (listing.Status != ListingStatus.Approved)
        {
            listing.Status = ListingStatus.Approved;
            listing.UpdatedAt = DateTime.UtcNow;
        }
        return await dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> RejectListing(Guid listingId)
    {
        var listing = await dbContext.Listings.FirstOrDefaultAsync(l => l.ListingId == listingId);
        if (listing == null) return false;
        if (listing.Status != ListingStatus.Reject)
        {
            listing.Status = ListingStatus.Reject;
            listing.UpdatedAt = DateTime.UtcNow;
        }
        return await dbContext.SaveChangesAsync() > 0;
    }

    public async Task<List<Data.Models.Listing>> GetReviewedListings()
    {
        return await dbContext.Listings
            .AsNoTracking()
            .Include(l => l.Images)
            .Where(l => l.Status != ListingStatus.Pending)
            .OrderBy(l => l.CreatedAt)
            .ToListAsync(); 
    }

    public async Task<List<Data.Models.Listing>> GetListingsByStatus(string status)
    {
        var historyListings =  dbContext.Listings
            .AsNoTracking()
            .Include(l => l.Images)
            .Where(l => l.Status != ListingStatus.Pending);

        switch (status)
        {
            case "Approved":
                return await historyListings.Where(x => x.Status == ListingStatus.Approved).ToListAsync();
            case "Rejected":
                return await historyListings.Where(x => x.Status == ListingStatus.Reject).ToListAsync();
            case "Sold":
                return await historyListings.Where(x => x.Status == ListingStatus.Sold).ToListAsync();
            case "All":
                 return await historyListings.ToListAsync();
            default:
                return await historyListings.ToListAsync();
        }
    }
    
    public async Task<List<Data.Models.Listing>> GetAllListingsRaw()
    {
        return await dbContext.Listings.AsNoTracking().ToListAsync();
    }

}