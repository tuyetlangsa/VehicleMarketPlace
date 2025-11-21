using ElectricVehicleManagement.Data.Abstraction;
using ElectricVehicleManagement.Data.Models.Dto;
using ElectricVehicleManagement.Data.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace ElectricVehicleManagement.Service.User;

public class UserService(IDbContext dbContext) : IUserService
{
    public async Task<Data.Models.User?> GetOrAddUser(string email, string? phone, string fullName, string? address)
    {
        
        var user = await dbContext.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Email == email);
        if (user is null)
        {
            var newUser = new Data.Models.User()
            {
                Id = Guid.NewGuid(),
                Email = email,
                FullName = fullName,
                Phone = phone,
                Address = address,
                Status = true,
                Role = Role.Member
            };
            dbContext.Users.Add(newUser);
            await dbContext.SaveChangesAsync();
            return newUser;
        }

        return user;
    }

    public async Task<PagedResult<Data.Models.User>> GetPaged(int page, int pageSize, string? keyword)
    {

        var allUsers = await dbContext.Users
            .IgnoreQueryFilters()
            .AsNoTracking()
            .ToListAsync();
        
        var filtered = allUsers
            .Where(u => u.Role != Role.Administrator)
            .ToList();


        if (!string.IsNullOrWhiteSpace(keyword))
        {
            keyword = keyword.ToLower();
            filtered = filtered.Where(u =>
                u.Email.ToLower().Contains(keyword) ||
                (u.Phone != null && u.Phone.ToLower().Contains(keyword)) ||
                u.FullName.ToLower().Contains(keyword)
            ).ToList();
        }


        var totalItems = filtered.Count;
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var items = filtered
            .OrderBy(u => u.FullName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<Data.Models.User>
        {
            Items = items,
            Page = page,
            TotalPages = totalPages,
            TotalItems = totalItems
        };
    }
    
    public async Task UpdateStatus(Guid userId, bool newStatus)
    {
        var user = await dbContext.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return;
    
        user.Status = newStatus;
        await dbContext.SaveChangesAsync();
    }
    
    public async Task<List<Data.Models.User>> GetAllUsers()
    {
        var allUsers = await dbContext.Users
            .IgnoreQueryFilters()
            .AsNoTracking()
            .ToListAsync();
        var filtered = allUsers
            .Where(u => u.Role != Role.Administrator)
            .ToList();
        return filtered;
    }

    public async Task<Data.Models.User?> GetUserById(Guid userId)
    {
        return await dbContext.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId);
    }
    
    public async Task Update(Data.Models.User user)
    {
        dbContext.Users.Update(user);  
        await dbContext.SaveChangesAsync();
    }
    
}