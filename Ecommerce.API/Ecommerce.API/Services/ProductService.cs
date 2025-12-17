using Ecommerce.API.Data;
using Ecommerce.API.DTOs.Products;
using Ecommerce.API.Entities.Products;

namespace Ecommerce.API.Services
{
	public class ProductService : IProductService
	{
		private readonly ApplicationDbContext _context;

		public ProductService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<Product> CreateProductAsync(CreateProductDto createProductDto)
		{
			// Simple mapping from DTO to entity
			var product = new Product
			{
				Name = createProductDto.Name,
				Description = createProductDto.Description,
				Category = createProductDto.Category,
				Tags = createProductDto.Tags ?? string.Empty,
				OriginalPrice = createProductDto.OriginalPrice,
				DiscountPrice = createProductDto.DiscountPrice,
				Stock = createProductDto.Stock,
				ShopId = createProductDto.ShopId,
				Status = ProductStatus.Draft, // Default status
				CreatedAt = DateTime.UtcNow
			};

			_context.Products.Add(product);
			await _context.SaveChangesAsync();

			return product;
		}
	}
}