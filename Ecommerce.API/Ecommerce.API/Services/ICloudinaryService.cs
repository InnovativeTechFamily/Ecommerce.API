using CloudinaryDotNet.Actions;
using Ecommerce.API.DTOs.Cloudinary;
using Ecommerce.API.Entities;

namespace Ecommerce.API.Services
{
    public interface ICloudinaryService
    {
        Task<ImageUploadResult> UploadImageAsync(string imageData, string folder = "avatars");
        Task<DeletionResult> DeleteImageAsync(string publicId);
        Task<CloudinaryUploadResult> UploadBase64ImageAsync(string base64Image, string folder);

        Task<Media> UploadBase64ImageAndCreateMediaAsync(
            string base64Image,
            string folder,
            string? entityType = null,  // "Product", "User"
            string? entityId = null);
       // Task<List<Media>> UpdateEntityImagesAsync(
        //List<string> base64Images, string folder, string entityType, string entityId);

    }
}
