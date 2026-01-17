using System.ComponentModel.DataAnnotations;

namespace Ecommerce.API.DTOs.Products
{
	public class DeleteProductsDto
	{
		[Required]
		public List<string> ProductIds { get; set; } = new();
	}
}
