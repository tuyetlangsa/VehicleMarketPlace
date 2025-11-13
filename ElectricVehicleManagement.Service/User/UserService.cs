using ElectricVehicleManagement.Data.Abstraction;
using ElectricVehicleManagement.Data.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace ElectricVehicleManagement.Service.User;

public class UserService(IDbContext dbContext) : IUserService
{
    public async Task<Data.Models.User?> GetOrAddUser(string email, string? phone, string fullName, string? address)
    {
        
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
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
}