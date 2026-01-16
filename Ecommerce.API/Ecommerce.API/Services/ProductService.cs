using Ecommerce.API.Data;
using Ecommerce.API.DTOs.Products;
using Ecommerce.API.Entities.Products;
using Ecommerce.API.Utils;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Services
{
	public class ProductService : IProductService
	{
		private readonly ApplicationDbContext _context;

		public ProductService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<Product> CreateProductAsync(Guid sellerId, CreateProductDto createProductDto)
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
				ShopId = sellerId,
				Status = ProductStatus.Draft, // Default status
				CreatedAt = DateTime.UtcNow
			};

			_context.Products.Add(product);
			await _context.SaveChangesAsync();

			return product;
		}
		public async Task<ProductResponseDto> GetProductByIdAsync(Guid sellerId, string productId)
		{
			var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId && p.ShopId== sellerId); // Fixed parameter name

			if (product == null)
			{
				throw new KeyNotFoundException($"Product with ID {productId} not found");
			}

			// Map Product entity to ProductResponseDto
			return new ProductResponseDto
			{
				Id = product.Id,
				Name = product.Name,
				Description = product.Description,
				Category = product.Category,
				Tags = product.Tags,
				OriginalPrice = product.OriginalPrice,
				DiscountPrice = product.DiscountPrice,
				Stock = product.Stock,
				ShopId = sellerId,
				Status = (int)product.Status,
				CreatedAt = product.CreatedAt
			};
		}



		public async Task<List<ProductResponseDto>> GetAllProductsAsync()
		{
			var products = await _context.Products
				.OrderByDescending(p => p.CreatedAt) // Latest first
				.Select(p => new ProductResponseDto
				{
					Id = p.Id,
					Name = p.Name,
					Description = p.Description,
					Category = p.Category,
					Tags = p.Tags,
					OriginalPrice = p.OriginalPrice,
					DiscountPrice = p.DiscountPrice,
					Stock = p.Stock,
					ShopId = p.ShopId,
					Status = (int)p.Status,
					CreatedAt = p.CreatedAt
				})
				.ToListAsync();

			return products;
		}
		public async Task<ProductResponseDto> UpdateProductAsync(string productId, CreateProductDto updateProductDto)
		{
			var product = await _context.Products
				.FirstOrDefaultAsync(p => p.Id == productId);

			if (product == null)
			{
				throw new KeyNotFoundException($"Product with ID {productId} not found");
			}

			// Update only the fields that are provided (non-null/empty)
			if (!string.IsNullOrEmpty(updateProductDto.Name))
				product.Name = updateProductDto.Name;

			if (!string.IsNullOrEmpty(updateProductDto.Description))
				product.Description = updateProductDto.Description;

			if (!string.IsNullOrEmpty(updateProductDto.Category))
				product.Category = updateProductDto.Category;

			if (updateProductDto.Tags != null)
				product.Tags = updateProductDto.Tags;

			if (updateProductDto.OriginalPrice.HasValue)
				product.OriginalPrice = updateProductDto.OriginalPrice.Value;

			// Always update discount price if provided (it's required in DTO)
			product.DiscountPrice = updateProductDto.DiscountPrice;

			if (updateProductDto.Stock > 0) // Assuming stock can't be negative
				product.Stock = updateProductDto.Stock;

			//if (updateProductDto.ShopId)
				//product.ShopId = updateProductDto.ShopId;

			// Update timestamp
			product.UpdatedAt = DateTime.UtcNow;

			// Validate price logic
			if (product.OriginalPrice.HasValue &&
				product.DiscountPrice > product.OriginalPrice.Value)
			{
				throw new InvalidOperationException("Discount price cannot be greater than original price");
			}

			await _context.SaveChangesAsync();

			return new ProductResponseDto
			{
				Id = product.Id,
				Name = product.Name,
				Description = product.Description,
				Category = product.Category,
				Tags = product.Tags,
				OriginalPrice = product.OriginalPrice,
				DiscountPrice = product.DiscountPrice,
				Stock = product.Stock,
				ShopId = product.ShopId,
				Status = (int)product.Status,
				CreatedAt = product.CreatedAt
			};
		}

		public async Task DeleteProductAsync(string productId, Guid sellerShopId)
		{
			var product = await _context.Products
				.FirstOrDefaultAsync(p => p.Id == productId);

			if (product == null)
				throw new KeyNotFoundException("Product not found");

			// 🔐 Seller ownership check
			if (product.ShopId != sellerShopId)
				throw new UnauthorizedAccessException(
					"You are not allowed to delete this product"
				);

			_context.Products.Remove(product);
			await _context.SaveChangesAsync();
		}


		public async Task<List<ProductResponseDto>> GetProductByShopAsync(Guid sellerId)
		{

			var products = await _context.Products
				.OrderByDescending(p => p.CreatedAt) // Latest first
				.Select(p => new ProductResponseDto
				{
					Id = p.Id,
					Name = p.Name,
					Description = p.Description,
					Category = p.Category,
					Tags = p.Tags,
					OriginalPrice = p.OriginalPrice,
					DiscountPrice = p.DiscountPrice,
					Stock = p.Stock,
					ShopId = p.ShopId,
					Status = (int)p.Status,
					CreatedAt = p.CreatedAt
				})
				.ToListAsync();

			return products;
		}

		public async Task DeleteProductsAsync(List<string> productIds, Guid shopId)
		{
			if (productIds == null || productIds.Count == 0)
				throw new ErrorHandler("Product IDs are required", 400);

			var products = await _context.Products
				.Where(p => productIds.Contains(p.Id) && p.ShopId == shopId)
				.ToListAsync();

			if (products.Count == 0)
				throw new ErrorHandler("No products found for deletion", 404);

			_context.Products.RemoveRange(products);
			await _context.SaveChangesAsync();
		}


	}
}