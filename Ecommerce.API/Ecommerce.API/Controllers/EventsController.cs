using CloudinaryDotNet.Actions;
using Ecommerce.API.Data;
using Ecommerce.API.DTOs.Events;
using Ecommerce.API.Entities;
using Ecommerce.API.Entities.Event;
using Ecommerce.API.Middleware;
using Ecommerce.API.Services;
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

            // 1️⃣ Validate ShopId (Node.js: Shop.findById)
            var shopExists = await _context.Shops
                .AnyAsync(s => s.Id.ToString() == createEventDto.ShopId);

            if (!shopExists)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Shop Id is invalid!"
                });
            }

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
                ShopId = createEventDto.ShopId,
                Start_Date = createEventDto.Start_Date,
                Finish_Date = createEventDto.Finish_Date,
                Status = "Running"
            };

            // 4️⃣ Upload images (Node.js: cloudinary.uploader.upload)
            foreach (var base64 in createEventDto.Images)
            {
                var media = await _cloudinary.UploadBase64ImageAndCreateMediaAsync(
                    base64Image: base64,
                    folder: "events",
                    entityType: "Event",
                    entityId: null
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
            var events = await _context.Events
                .Where(e => e.ShopId == shopId)
                .Include(e => e.Media)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

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
