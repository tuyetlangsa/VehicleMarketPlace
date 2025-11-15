using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectricVehicleManagement.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ElectricVehicleManagement.Data.Abstraction
{
    public interface IDbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Listing> Listings { get; set; }
        public DbSet<ListingImage> ListingImages { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
