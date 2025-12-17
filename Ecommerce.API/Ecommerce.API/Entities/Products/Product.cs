using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.API.Entities.Products
{
	public class Product
	{
		[Key]
		public int Id { get; set; }

		[Required(ErrorMessage = "Please enter your product name!")]
		[StringLength(200)]
		public string Name { get; set; }

		[Required(ErrorMessage = "Please enter your product description!")]
		[Column(TypeName = "nvarchar(max)")]
		public string Description { get; set; }

		[Required(ErrorMessage = "Please enter your product category!")]
		[StringLength(100)]
		public string Category { get; set; }

		[StringLength(500)]
		public string Tags { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal? OriginalPrice { get; set; }

		[Required(ErrorMessage = "Please enter your product price!")]
		[Column(TypeName = "decimal(18,2)")]
		public decimal DiscountPrice { get; set; }

		[Required(ErrorMessage = "Please enter your product stock!")]
		public int Stock { get; set; }

		[Required]
		[StringLength(50)]
		public string ShopId { get; set; }

		public ProductStatus Status { get; set; } = ProductStatus.Draft;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}

	public enum ProductStatus
	{
		None = 0,
		Draft = 1,
		Active = 2,
		Archived = 3,
		Discontinued = 4
	}
}