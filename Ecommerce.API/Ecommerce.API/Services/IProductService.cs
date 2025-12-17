using Ecommerce.API.DTOs.Products;
using Ecommerce.API.Entities.Products;

namespace Ecommerce.API.Services
{
	public interface IProductService
	{
		Task<Product> CreateProductAsync(CreateProductDto createProductDto);
	}
}