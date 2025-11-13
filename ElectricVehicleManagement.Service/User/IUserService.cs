namespace ElectricVehicleManagement.Service.User;

public interface IUserService
{
    public Task<Data.Models.User?> GetOrAddUser(string email, string? phone, string fullName, string? address);
}