using System.ComponentModel.DataAnnotations;

namespace Ecommerce.API.DTOs.Products
{
	public class CreateProductDto
	{
		[Required(ErrorMessage = "Product name is required")]
		[StringLength(200, MinimumLength = 3, ErrorMessage = "Product name must be between 3 and 200 characters")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Product description is required")]
		public string Description { get; set; }

		[Required(ErrorMessage = "Product category is required")]
		[StringLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
		public string Category { get; set; }

		[StringLength(500, ErrorMessage = "Tags cannot exceed 500 characters")]
		public string Tags { get; set; }

		[Range(0, double.MaxValue, ErrorMessage = "Original price cannot be negative")]
		public decimal? OriginalPrice { get; set; }

		[Required(ErrorMessage = "Discount price is required")]
		[Range(0.01, double.MaxValue, ErrorMessage = "Discount price must be greater than 0")]
		public decimal DiscountPrice { get; set; }

		[Required(ErrorMessage = "Stock is required")]
		[Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
		public int Stock { get; set; }

		[Required(ErrorMessage = "Shop ID is required")]
	//	[StringLength(50, ErrorMessage = "Shop ID cannot exceed 50 characters")]
		public Guid ShopId { get; set; }

		public List<string>	Images { get; set; } = new List<string>();
    }
}