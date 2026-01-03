using Ecommerce.API.Entities;

namespace Ecommerce.API.Services
{
    public interface IMediaService
    {
        Task<Media> AddAsync(Media media);
        Task<Media?> GetByIdAsync(string id);
        Task<IReadOnlyList<Media>> GetByEntityAsync(string entityType, string entityId);
        Task<IReadOnlyList<Media>> GetByFolderAsync(string folder);
        Task DeleteAsync(string id);
        Task DeleteByEntityAsync(string entityType, string entityId);

        // High-level: replace all images for an entity (uses Cloudinary + DB)
        Task<IReadOnlyList<Media>> ReplaceEntityImagesAsync(
            List<string> base64Images,
            string folder,
            string entityType,
            string entityId);
    }
}
