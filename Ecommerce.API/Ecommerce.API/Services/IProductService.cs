using Ecommerce.API.DTOs.Products;
using Ecommerce.API.Entities.Products;

namespace Ecommerce.API.Services
{
	public interface IProductService
	{
		Task<Product> CreateProductAsync(Guid sellerId, CreateProductDto createProductDto);
		Task<ProductResponseDto> GetProductByIdAsync(Guid sellerId, int productId);
	    Task<List<ProductResponseDto>> GetAllProductsAsync();
		Task<ProductResponseDto> UpdateProductAsync(int productId, CreateProductDto updateProductDto);
		Task<bool> DeleteProductAsync(int productId);

	}
}