using CloudinaryDotNet.Actions;
using Ecommerce.API.Data;
using Ecommerce.API.DTOs.Events;
using Ecommerce.API.DTOs.Shop;
using Ecommerce.API.Entities;
using Ecommerce.API.Entities.Event;
using Ecommerce.API.Entities.Products;
using Ecommerce.API.Entities.Shops;
using Ecommerce.API.Mappings;
using Ecommerce.API.Middleware;
using Ecommerce.API.Services;
using Ecommerce.API.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class EventsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ICloudinaryService _cloudinary;

        public EventsController(
            ApplicationDbContext context,
            ICloudinaryService cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }

        [HttpPost]
        [IsSeller]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto createEventDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var seller = HttpContext.Items["Seller"] as Shop;

            // 2️⃣ Validate Images (Node.js logic)
            if (createEventDto.Images == null || !createEventDto.Images.Any())
            {
                return BadRequest(new
                {
                    success = false,
                    message = "At least one image is required"
                });
            }

            // 3️⃣ Create Event entity
            var evt = new Event
            {
                Name = createEventDto.Name,
                Description = createEventDto.Description,
                Category = createEventDto.Category,
                Tags = createEventDto.Tags,
                OriginalPrice = createEventDto.OriginalPrice,
                DiscountPrice = createEventDto.DiscountPrice,
                Stock = createEventDto.Stock,
                ShopId = seller.Id.ToString(),
                Start_Date = createEventDto.Start_Date,
                Finish_Date = createEventDto.Finish_Date,
                Status = ProductStatus.Draft.ToString()
            };

            // 4️⃣ Upload images (Node.js: cloudinary.uploader.upload)
            foreach (var base64 in createEventDto.Images)
            {
                var media = await _cloudinary.UploadBase64ImageAndCreateMediaAsync(
                    base64Image: base64,
                    folder: EntityType.Events,
                    entityType: EntityType.Events,
                    entityId: evt.Id
                );

                evt.Media.Add(media);
            }

            // 5️⃣ Save Event
            _context.Events.Add(evt);
            await _context.SaveChangesAsync();

            return StatusCode(StatusCodes.Status201Created, new
            {
                success = true,
                events = evt
            });
        }




        [HttpGet("shop/{shopId}")]
        public async Task<IActionResult> GetEventsByShop(string shopId)
        {
            var res = await _context.Events
                .Where(e => e.ShopId == shopId)
                .Include(e => e.Media)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            var events = EventMapper.ToDtoList(res);
            Guid sellerId = Guid.Parse(shopId);
            var shop = await _context.Shops.FindAsync(sellerId);
            events.Select(e =>
            {
                e.Shop = new ShopDto
                {
                    Id = shop.Id.ToString(),
                    Name = shop.Name,
                    AvatarUrl = shop.AvatarUrl,
                    Description = shop.Description,
                    CreatedAt = shop.CreatedAt
                };
                return e;
            }).ToList();
            return Ok(new
            {
                success = true,
                events
            });
        }


        [HttpGet("running")]
        public async Task<IActionResult> GetRunningEvents()
        {
            var events = await _context.Events
                .Where(e => e.Status == "Running")
                .Include(e => e.Media)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            return Ok(new
            {
                success = true,
                events
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            var res = await _context.Events
                .Include(e => e.Media)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
            var events = EventMapper.ToDtoList(res);

            var allShop = await _context.Shops.ToListAsync();

            events.Select(e =>
            {
                var shop = allShop.FirstOrDefault(s => s.Id.ToString() == e.ShopId);
                if (shop != null)
                {
                    e.Shop = new ShopDto
                    {
                        Id = shop.Id.ToString(),
                        Name = shop.Name,
                        AvatarUrl = shop.AvatarUrl,
                        Description = shop.Description,
                        CreatedAt = shop.CreatedAt
                    };
                }
                return e;
            }).ToList();
           

            return Ok(new
            {
                success = true,
                events
            });
        }

        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEventById(string eventId)
        {
            var evt = await _context.Events
                .Include(e => e.Media)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (evt == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Event not found with the given id."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Event fetched successfully.",
                data = evt
            });
        }

        [HttpDelete("{eventId}")]
        public async Task<IActionResult> DeleteEvent(string eventId)
        {
            var evt = await _context.Events
                .Include(e => e.Media)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (evt == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Event is not found with this id"
                });
            }

            foreach (var media in evt.Media)
            {
                await _cloudinary.DeleteImageAsync(media.PublicId);
            }

            _context.Media.RemoveRange(evt.Media);
            _context.Events.Remove(evt);
            await _context.SaveChangesAsync();

            return StatusCode(StatusCodes.Status201Created, new
            {
                success = true,
                message = "Event Deleted successfully!"
            });
        }


        [HttpGet("admin")]
        [IsAuthenticated]
        [IsAdmin("Admin")]
        public async Task<IActionResult> GetAllEventsForAdmin()
        {
            var events = await _context.Events
                .Include(e => e.Media)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            return Ok(new
            {
                success = true,
                events
            });
        }

    }
}
