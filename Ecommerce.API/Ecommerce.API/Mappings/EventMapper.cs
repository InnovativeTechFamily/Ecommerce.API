using Ecommerce.API.DTOs.Events;
using Ecommerce.API.Utils;

namespace Ecommerce.API.Mappings
{
    public static class EventMapper
    {
        // Entity to DTO mappings
        public static EventDto ToDto(this Entities.Event.Event entity)
        {
            if (entity == null) return null;

            return new EventDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Category = entity.Category,
                Start_Date = entity.Start_Date,
                Finish_Date = entity.Finish_Date,
                Status = entity.Status,
                Tags = entity.Tags,
                OriginalPrice = entity.OriginalPrice,
                DiscountPrice = entity.DiscountPrice,
                Stock = entity.Stock,
                ShopId = entity.ShopId,
                SoldOut = entity.SoldOut,
                CreatedAt = entity.CreatedAt,
                Images = entity.Media?.Select(m => m.ToDto()).ToList() ?? new List<MediaDto>()
            };
        }

        public static List<EventDto> ToDtoList(this IEnumerable<Entities.Event.Event> entities)
        {
            return entities.Select(e => e.ToDto()).ToList();
        }

        public static Entities.Event.Event ToEntity(this CreateEventDto dto)
        {
            return new Entities.Event.Event
            {
                Id = Entities.Event.Event.NewId(),
                Name = dto.Name,
                Description = dto.Description,
                Category = dto.Category,
                Start_Date = dto.Start_Date,
                Finish_Date = dto.Finish_Date,
                Status = "Running",
                Tags = dto.Tags,
                OriginalPrice = dto.OriginalPrice,
                DiscountPrice = dto.DiscountPrice,
                Stock = dto.Stock,
               // ShopId = dto.ShopId,
                SoldOut = 0,
                CreatedAt = DateTime.UtcNow
            };
        }

        public static void UpdateEntity(this UpdateEventDto dto, Entities.Event.Event entity)
        {
            if (!string.IsNullOrEmpty(dto.Name))
                entity.Name = dto.Name;

            if (!string.IsNullOrEmpty(dto.Description))
                entity.Description = dto.Description;

            if (!string.IsNullOrEmpty(dto.Category))
                entity.Category = dto.Category;

            if (dto.StartDate.HasValue)
                entity.Start_Date = dto.StartDate.Value;

            if (dto.FinishDate.HasValue)
                entity.Finish_Date = dto.FinishDate.Value;

            if (!string.IsNullOrEmpty(dto.Status))
                entity.Status = dto.Status;

            if (dto.Tags != null)
                entity.Tags = dto.Tags;

            if (dto.OriginalPrice.HasValue)
                entity.OriginalPrice = dto.OriginalPrice.Value;

            if (dto.DiscountPrice.HasValue)
                entity.DiscountPrice = dto.DiscountPrice.Value;

            if (dto.Stock.HasValue)
                entity.Stock = dto.Stock.Value;

            if (dto.SoldOut.HasValue)
                entity.SoldOut = dto.SoldOut.Value;
        }
    }

    public static class MediaMapper
    {
        public static MediaDto ToDto(this Entities.Media entity)
        {
            if (entity == null) return null;

            return new MediaDto
            {
                Id = entity.Id,
                PublicId = entity.PublicId,
                Url = entity.Url,
                Folder = entity.Folder,
                FileName = entity.FileName,
                EntityType = entity.EntityType,
                EntityId = entity.EntityId,
                CreatedAt = entity.CreatedAt
            };
        }

        //public static Entities.Media ToEntity(this CreateMediaDto dto)
        //{
        //    return new Entities.Media
        //    {
        //        Id = Entities.Media.NewId(),
        //        PublicId = dto.PublicId,
        //        Url = dto.Url,
        //        Folder = dto.EntityType == "Product" ? CloudinaryFolders.Products :
        //                dto.EntityType == "User" ? CloudinaryFolders.Users :
        //                CloudinaryFolders.Shops,
        //        FileName = dto.FileName,
        //        EntityType = dto.EntityType,
        //        EntityId = dto.EntityId,
        //        CreatedAt = DateTime.UtcNow
        //    };
        //}
    }
}
