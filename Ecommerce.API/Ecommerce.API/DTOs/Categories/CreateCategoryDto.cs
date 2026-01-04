using System.ComponentModel.DataAnnotations;

namespace Ecommerce.API.DTOs.Categories
{
	public class CreateCategoryDto
	{
		[Required]
		public string Name { get; set; }
		[Required]
		public string Code { get; set; }
	}
}
