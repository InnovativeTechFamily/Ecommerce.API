using Ecommerce.API.Data;
using Ecommerce.API.DTOs.Categories;
using Ecommerce.API.Entities;
using Ecommerce.API.Utils;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Services
{
	public class CategoryService : ICategoryService
	{
		private readonly ApplicationDbContext _context;
		public CategoryService(ApplicationDbContext context) 
		{
		 _context = context;
		}

		public async Task CreateAsync(CreateCategoryDto createCategoryDto)
		{
			var exists = await _context.Categories
				.AnyAsync(x => x.Code == createCategoryDto.Code);

			if (exists)
			{
				throw new ErrorHandler("Already Category exist", 400);
			}

			var category = new Category
			{
				Name = createCategoryDto.Name,
				Code = createCategoryDto.Code
			};

			_context.Categories.Add(category);
			await _context.SaveChangesAsync();
		}


		public async Task<List<CategoryResponseDto>> GetAllAsync()
		{
			var categories = await _context.Categories
				.OrderByDescending(x => x.Name) // Or any order you prefer
				.Select(x => new CategoryResponseDto
				{
					Name = x.Name,
					Code = x.Code
				})
				.ToListAsync();

			return categories;
		}


		public async Task DeleteAsync(string code)
		{
			// Find the category by code
			var category = await _context.Categories.FirstOrDefaultAsync(c => c.Code == code);

			if (category == null)
				throw new ErrorHandler("Category not found", 404);

			_context.Categories.Remove(category);
			await _context.SaveChangesAsync();
		}

	}
}
