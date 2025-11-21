using ElectricVehicleManagement.Data.Models.Dto;

namespace ElectricVehicleManagement.Service.User;

public interface IUserService
{
    public Task<Data.Models.User?> GetOrAddUser(string email, string? phone, string fullName, string? address);

    public Task<PagedResult<Data.Models.User>> GetPaged(int page, int pageSize, string? keyword);

    public Task UpdateStatus(Guid userId, bool newStatus);

    public Task<List<Data.Models.User>> GetAllUsers();
    
    public Task<Data.Models.User?> GetUserById(Guid userId);
    
    public Task Update(Data.Models.User user);

}