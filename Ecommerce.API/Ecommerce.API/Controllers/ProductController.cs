using Ecommerce.API.DTOs.Products;
using Ecommerce.API.Entities.Shops;
using Ecommerce.API.Middleware;
using Ecommerce.API.Services;
using Ecommerce.API.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductsController : ControllerBase
	{
		private readonly IProductService _productService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ILogger<ProductsController> _logger;
        private readonly IMediaService _mediaService;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger, ICloudinaryService cloudinaryService, IMediaService mediaService)
        {
            _productService = productService;
            _logger = logger;
            _cloudinaryService = cloudinaryService;
            _mediaService = mediaService;
        }

        // Fix for CS0029 and CS8602 in CreateProduct method
        [HttpPost("create-product")]
        [IsSeller]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Validation failed",
                        errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                            .ToList()
                    });
                }
                // Business rule: Discount price should not be greater than original price
                if (createProductDto.OriginalPrice.HasValue &&
                    createProductDto.DiscountPrice > createProductDto.OriginalPrice.Value)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Discount price cannot be greater than original price"
                    });
                }
                var seller = HttpContext.Items["Seller"] as Shop;
                if (seller == null)
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Seller not found in context."
                    });
                }

              

                var product = await _productService.CreateProductAsync(seller.Id, createProductDto);
                //upload product image 
                if (createProductDto.Images.Count > 0)
                {
                    foreach (var img in createProductDto.Images)
                    {
                        // The method returns Media, not CloudinaryUploadResult, so just await it for side effects
                        await _cloudinaryService.UploadBase64ImageAndCreateMediaAsync(img, CloudinaryFolders.Products, EntityType.Products, product.Id.ToString());
                    }
                }
                return Ok(new
                {
                    success = true,
                    message = "Product created successfully",
                    data = new
                    {
                        id = product.Id,
                        name = product.Name,
                        description = product.Description,
                        category = product.Category,
                        tags = product.Tags,
                        originalPrice = product.OriginalPrice,
                        discountPrice = product.DiscountPrice,
                        stock = product.Stock,
                        shopId = seller.Id,
                        status = product.Status,
                        createdAt = product.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error creating product",
                    error = ex.Message
                });
            }
        }

			// GET: api/products/{id}
		[HttpGet("{id}")]
        [IsSeller]
        public async Task<IActionResult> GetProduct(string id)
			{
				try
				{
					// Validate ID
					if (string.IsNullOrEmpty(id))
					{
						return BadRequest(new
						{
							success = false,
							message = "Invalid product ID. ID must be greater than 0."
						});
					}
                var seller = HttpContext.Items["Seller"] as Shop;
                
                var product = await _productService.GetProductByIdAsync(seller.Id,id);
				var media = await _mediaService.GetByEntityAsync(EntityType.Products, id.ToString());


				List<ProductImageResponseDto> img = new List<ProductImageResponseDto>();
                foreach (var item in media)
                {
                   var im= new ProductImageResponseDto { 
					Id=item.Id,
					PublicId=item.PublicId,
					Url=item.Url
					};
                   // product.Images.AddRange(im);
					img.Add(im);
                }

				product.Images = img;


                //	product.Images
                return Ok(new
					{
						success = true,
						message = "Product retrieved successfully",
						data = product
					});
				}
				catch (KeyNotFoundException ex)
				{
					return NotFound(new
					{
						success = false,
						message = ex.Message
					});
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error retrieving product with ID {Id}", id);
					return StatusCode(500, new
					{
						success = false,
						message = "Error retrieving product",
						error = ex.Message
					});
				}
			}
		[HttpGet("GetAllProduct")]
       
        public async Task<IActionResult> GetAllProducts()
		{
			try
			{
				var products = await _productService.GetAllProductsAsync();
				var allMedia = await _mediaService.GetByFolderAsync(CloudinaryFolders.Products);
                List<ProductImageResponseDto> img = new List<ProductImageResponseDto>();

			 foreach(var p in products)
				{
					p.Images = new List<ProductImageResponseDto>();
                    foreach (var item in allMedia)
                    {
                        if (item.EntityId == p.Id.ToString())
                        {
                            var im = new ProductImageResponseDto
                            {
                                Id = item.Id,
                                PublicId = item.PublicId,
                                Url = item.Url
                            };
                            p.Images.Add(im);
                        }
                    }
                }

                return Ok(new
				{
					success = true,
					message = "Products retrieved successfully",
                    products = products,
                    productCount = products.Count
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving products");
				return StatusCode(500, new
				{
					success = false,
					message = "Error retrieving products",
					error = ex.Message
				});
			}
		}
		// PUT: api/products/{id}
		[HttpPut("{id}")]
        [IsSeller]
        public async Task<IActionResult> UpdateProduct(string id, [FromBody] CreateProductDto updateProductDto)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(new
					{
						success = false,
						message = "Validation failed",
						errors = ModelState.Values
							.SelectMany(v => v.Errors)
							.Select(e => e.ErrorMessage)
							.ToList()
					});
				}

				if (string.IsNullOrEmpty(id))
				{
					return BadRequest(new
					{
						success = false,
						message = "Invalid product ID"
					});
				}

				var updatedProduct = await _productService.UpdateProductAsync(id, updateProductDto);

				return Ok(new
				{
					success = true,
					message = "Product updated successfully",
					data = updatedProduct
				});
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(new
				{
					success = false,
					message = ex.Message
				});
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(new
				{
					success = false,
					message = ex.Message
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating product with ID {Id}", id);
				return StatusCode(500, new
				{
					success = false,
					message = "Error updating product",
					error = ex.Message
				});
			}
		}

		// DELETE: api/products/{id}
		[HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
		{
			try
			{
				if (string.IsNullOrEmpty(id))
				{
					return BadRequest(new
					{
						success = false,
						message = "Invalid product ID"
					});
				}

				var result = await _productService.DeleteProductAsync(id);

				return Ok(new
				{
					success = true,
					message = "Product deleted successfully"
				});
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(new
				{
					success = false,
					message = ex.Message
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting product with ID {Id}", id);
				return StatusCode(500, new
				{
					success = false,
					message = "Error deleting product",
					error = ex.Message
				});
			}
		}
	}
}	