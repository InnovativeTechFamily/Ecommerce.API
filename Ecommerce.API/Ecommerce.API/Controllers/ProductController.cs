using Microsoft.AspNetCore.Mvc;
using Ecommerce.API.DTOs.Products;
using Ecommerce.API.Services;

namespace Ecommerce.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductsController : ControllerBase
	{
		private readonly IProductService _productService;

		public ProductsController(IProductService productService)
		{
			_productService = productService;
		}

		[HttpPost("create-product")]
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

				var product = await _productService.CreateProductAsync(createProductDto);

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
						shopId = product.ShopId,
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
	}
}