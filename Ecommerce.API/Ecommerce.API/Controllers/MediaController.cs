using Ecommerce.API.DTOs.Comman;
using Ecommerce.API.Services;
using Ecommerce.API.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly ICloudinaryService _cloudinaryService;

        public MediaController(ICloudinaryService cloudinaryService)
        {
            _cloudinaryService = cloudinaryService;
        }

        // POST: /api/media/upload-image
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage([FromBody] UploadImageRequestDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Image))
                {
                    return BadRequest(new { error = "Image is required" });
                }

                // Upload to Cloudinary in the "products" folder
                var result = await _cloudinaryService.UploadBase64ImageAsync(
                    dto.Image,
                    CloudinaryFolders.Products
                );

                return Ok(new
                {
                    public_id = result.PublicId,
                    imageUrl = result.ImageUrl
                });
            }
            catch (Exception ex)
            {
                // Equivalent to your catch block with 500
                Console.Error.WriteLine($"Error uploading image to Cloudinary: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    error = "Internal Server Error"
                });
            }
        }
    }
}
