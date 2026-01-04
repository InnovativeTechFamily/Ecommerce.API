using Ecommerce.API.DTOs.Categories;
using Ecommerce.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{
	[ApiController]
	[Route("api/category")]
	public class CategoryController : ControllerBase
	{
		private readonly ICategoryService _categoryService;

		public CategoryController(ICategoryService categoryService)
		{
			_categoryService = categoryService;
		}


		[HttpPost("CreateCategory")]
		public async Task<IActionResult> Create([FromBody] CreateCategoryDto createCategoryDto)
		{
			await _categoryService.CreateAsync(createCategoryDto);
			return StatusCode(201, new
			{
				success = true,
				message = "Product created successfully"
			});
		}


		[HttpGet("get-categories")]
		public async Task<IActionResult> GetAll()
		{
			var categories = await _categoryService.GetAllAsync();

			return Ok(new
			{
				success = true,
				data = categories
			});
		}


		[HttpDelete("delete-category/{code}")]
		public async Task<IActionResult> DeleteCategory(string code)
			{
			await _categoryService.DeleteAsync(code);

			return Ok(new
			{
				success = true,
				message = "Category deleted successfully"
			});
		}


	}
}
