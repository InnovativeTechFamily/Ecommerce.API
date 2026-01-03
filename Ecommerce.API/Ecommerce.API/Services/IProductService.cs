using Ecommerce.API.DTOs.Products;
using Ecommerce.API.Entities.Products;

namespace Ecommerce.API.Services
{
	public interface IProductService
	{
		Task<Product> CreateProductAsync(Guid SellerId,CreateProductDto createProductDto);
		Task<ProductResponseDto> GetProductByIdAsync(int productId);
	    Task<List<ProductResponseDto>> GetAllProductsAsync();
		Task<ProductResponseDto> UpdateProductAsync(int productId, CreateProductDto updateProductDto);
		Task<bool> DeleteProductAsync(int productId);

	}
}