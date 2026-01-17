using System.ComponentModel.DataAnnotations;

namespace Ecommerce.API.DTOs.Products
{
	public class CreateReviewDto
	{
		[Required]
		public string ProductId { get; set; }

		[Required]
		[Range(1, 5)]
		public int Rating { get; set; }

		public string Comment { get; set; }

		[Required]
		public string OrderId { get; set; }
	}
}
