using Ecommerce.API.DTOs.Products;
using Ecommerce.API.Entities.Products;

namespace Ecommerce.API.Services
{
	public interface IProductService
	{
		Task<Product> CreateProductAsync(Guid sellerId, CreateProductDto createProductDto);
		Task<ProductResponseDto> GetProductByIdAsync(Guid sellerId, string productId);
		Task<List<ProductResponseDto>> GetProductByShopAsync(Guid sellerId);
	    Task<List<ProductResponseDto>> GetAllProductsAsync();
		Task<ProductResponseDto> UpdateProductAsync(string productId, CreateProductDto updateProductDto);
		Task<bool> DeleteProductAsync(string productId);

	}
}