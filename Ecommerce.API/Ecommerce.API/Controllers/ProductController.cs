using Ecommerce.API.Data;
using Ecommerce.API.DTOs.Products;
using Ecommerce.API.Entities.Shops;
using Ecommerce.API.Middleware;
using Ecommerce.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductsController : ControllerBase
	{
		private readonly IProductService _productService;
		private readonly ILogger<ProductsController> _logger;

		public ProductsController(IProductService productService, ILogger<ProductsController> logger)
		{
			_productService = productService;
			_logger = logger;
		}

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
                var product = await _productService.CreateProductAsync(seller.Id,createProductDto);

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
			[HttpGet("{getByProductId}")]
			public async Task<IActionResult> GetProduct(int id)
			{
				try
				{
					// Validate ID
					if (id <= 0)
					{
						return BadRequest(new
						{
							success = false,
							message = "Invalid product ID. ID must be greater than 0."
						});
					}

					// Using service pattern
					var product = await _productService.GetProductByIdAsync(id);

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
        [IsSeller]
        public async Task<IActionResult> GetAllProducts()
		{
			try
			{
				var products = await _productService.GetAllProductsAsync();

				return Ok(new
				{
					success = true,
					message = "Products retrieved successfully",
					data = products,
					count = products.Count
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
		public async Task<IActionResult> UpdateProduct(int id, [FromBody] CreateProductDto updateProductDto)
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

				if (id <= 0)
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
		public async Task<IActionResult> DeleteProduct(int id)
		{
			try
			{
				if (id <= 0)
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