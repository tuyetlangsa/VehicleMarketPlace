using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace ElectricVehicleManagement.Service.Cloudinary;

public class CloudinaryService(CloudinaryDotNet.Cloudinary  cloudinary) : ICloudinaryService
{
    public async Task<string> UploadFileAsync(string filePath)
    {
        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(filePath),
            Folder = "image_uploads",
            UseFilename = true,
            UniqueFilename = true
        };
        
        var uploadResult = cloudinary.Upload(uploadParams);
        return uploadResult.SecureUrl.AbsoluteUri;
    }
}