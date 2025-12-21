using CloudinaryDotNet.Actions;

namespace Ecommerce.API.Services
{
    public interface ICloudinaryService
    {
        Task<ImageUploadResult> UploadImageAsync(string imageData, string folder = "avatars");
        Task<DeletionResult> DeleteImageAsync(string publicId);
    }
}
