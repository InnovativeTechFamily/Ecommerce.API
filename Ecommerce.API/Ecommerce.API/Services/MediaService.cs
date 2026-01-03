using Ecommerce.API.Data;
using Ecommerce.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Services
{
    public class MediaService : IMediaService
    {
        private readonly ApplicationDbContext _db;
        private readonly ICloudinaryService _cloudinary;

        public MediaService(ApplicationDbContext db, ICloudinaryService cloudinary)
        {
            _db = db;
            _cloudinary = cloudinary;
        }

        public async Task<Media> AddAsync(Media media)
        {
            _db.Media.Add(media);
            await _db.SaveChangesAsync();
            return media;
        }

        public async Task<Media?> GetByIdAsync(string id)
        {
            return await _db.Media.FindAsync(id);
        }

        public async Task<IReadOnlyList<Media>> GetByEntityAsync(string entityType, string entityId)
        {
            return await _db.Media
                .Where(m => m.EntityType == entityType && m.EntityId == entityId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Media>> GetByFolderAsync(string folder)
        {
            return await _db.Media
                .Where(m => m.Folder == folder)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var media = await _db.Media.FindAsync(id);
            if (media == null) return;

            // delete from Cloudinary
            await _cloudinary.DeleteImageAsync(media.PublicId);

            _db.Media.Remove(media);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteByEntityAsync(string entityType, string entityId)
        {
            var mediaList = await _db.Media
                .Where(m => m.EntityType == entityType && m.EntityId == entityId)
                .ToListAsync();

            foreach (var media in mediaList)
            {
                await _cloudinary.DeleteImageAsync(media.PublicId);
                _db.Media.Remove(media);
            }

            await _db.SaveChangesAsync();
        }

        // Replace all images for an entity (delete old, upload new, save)
        public async Task<IReadOnlyList<Media>> ReplaceEntityImagesAsync(
            List<string> base64Images,
            string folder,
            string entityType,
            string entityId)
        {
            // Delete old media
            var oldMedia = await _db.Media
                .Where(m => m.EntityType == entityType && m.EntityId == entityId)
                .ToListAsync();

            foreach (var media in oldMedia)
            {
                await _cloudinary.DeleteImageAsync(media.PublicId);
                _db.Media.Remove(media);
            }

            await _db.SaveChangesAsync();

            // Upload & save new media
            var newMediaList = new List<Media>();

            foreach (var base64 in base64Images)
            {
                var media = await _cloudinary.UploadBase64ImageAndCreateMediaAsync(
                    base64,
                    folder,
                    entityType,
                    entityId
                );

                newMediaList.Add(media);
            }

            return newMediaList;
        }
    }
}