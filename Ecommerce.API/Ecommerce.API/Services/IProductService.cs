using Ecommerce.API.DTOs.Products;
using Ecommerce.API.Entities.Products;
using Ecommerce.API.Entities.Users;

namespace Ecommerce.API.Services
{
	public interface IProductService
	{
		Task<Product> CreateProductAsync(Guid sellerId, CreateProductDto createProductDto);
		Task<ProductResponseDto> GetProductByIdAsync(Guid sellerId, string productId);
		Task<List<ProductResponseDto>> GetProductByShopAsync(Guid sellerId);
	    Task<List<ProductResponseDto>> GetAllProductsAsync();
		Task<ProductResponseDto> UpdateProductAsync(string productId, CreateProductDto updateProductDto);
		Task<List<ProductResponseDto>> GetProductsByShopAsync(Guid shopId);
		Task DeleteProductAsync(string productId, Guid shopId);
		Task DeleteProductsAsync(List<string> productIds, Guid shopId);

	}
}