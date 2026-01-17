using System.ComponentModel.DataAnnotations;

namespace Ecommerce.API.Entities.Products
{
	public class ProductReview
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public int UserId { get; set; }

		[Required]
		public string UserName { get; set; }

		[Required]
		public int Rating { get; set; }

		public string Comment { get; set; }

		[Required]
		public string ProductId { get; set; }

		public Product Product { get; set; }
	}
}
