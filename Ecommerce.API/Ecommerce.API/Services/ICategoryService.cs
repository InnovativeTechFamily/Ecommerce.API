using Ecommerce.API.DTOs.Categories;

namespace Ecommerce.API.Services
{
	public interface ICategoryService
	{
		Task CreateAsync(CreateCategoryDto createCategoryDto);
		Task<List<CategoryResponseDto>> GetAllAsync();
		Task DeleteAsync(string code);
	}
}
