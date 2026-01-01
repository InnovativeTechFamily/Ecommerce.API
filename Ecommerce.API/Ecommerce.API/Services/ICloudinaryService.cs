using CloudinaryDotNet.Actions;
using Ecommerce.API.DTOs.Cloudinary;

namespace Ecommerce.API.Services
{
    public interface ICloudinaryService
    {
        Task<ImageUploadResult> UploadImageAsync(string imageData, string folder = "avatars");
        Task<DeletionResult> DeleteImageAsync(string publicId);
        Task<CloudinaryUploadResult> UploadBase64ImageAsync(string base64Image, string folder);
    }
}
