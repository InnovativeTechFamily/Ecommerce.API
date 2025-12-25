using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Ecommerce.API.DTOs.Cloudinary;
using Microsoft.Extensions.Options;

namespace Ecommerce.API.Services
{

    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<CloudinaryService> _logger;

        public CloudinaryService(IOptions<CloudinarySettings> cloudinaryConfig,
            ILogger<CloudinaryService> logger)
        {
            var settings = cloudinaryConfig.Value;
            var account = new Account(
                settings.CloudName,
                settings.ApiKey,
                settings.ApiSecret);

            _cloudinary = new Cloudinary(account);
            _logger = logger;
        }

        public async Task<ImageUploadResult> UploadImageAsync(string imageData, string folder = "avatars")
        {
            try
            {
                // Check if imageData is base64 or URL
                var uploadParams = new ImageUploadParams();

                if (imageData.StartsWith("data:image"))
                {
                    // Base64 image
                    uploadParams.File = new FileDescription(imageData);
                }
                else if (imageData.StartsWith("http"))
                {
                    // URL
                    uploadParams.File = new FileDescription(imageData);
                }
                else
                {
                    // Assume it's a file path or other format
                    uploadParams.File = new FileDescription(imageData);
                }

                uploadParams.Folder = folder;
                uploadParams.Transformation = new Transformation()
                    .Width(500)
                    .Height(500)
                    .Crop("fill")
                    .Gravity("face");

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    _logger.LogError($"Cloudinary upload error: {uploadResult.Error.Message}");
                    throw new Exception($"Cloudinary upload failed: {uploadResult.Error.Message}");
                }

                return uploadResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image to Cloudinary");
                throw;
            }
        }

        public async Task<DeletionResult> DeleteImageAsync(string publicId)
        {
            try
            {
                var deleteParams = new DeletionParams(publicId);
                return await _cloudinary.DestroyAsync(deleteParams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image from Cloudinary");
                throw;
            }
        }
    }
}
