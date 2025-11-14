namespace ElectricVehicleManagement.Service.Cloudinary;

public interface ICloudinaryService
{
    public Task<string> UploadFileAsync(string filePath);
}