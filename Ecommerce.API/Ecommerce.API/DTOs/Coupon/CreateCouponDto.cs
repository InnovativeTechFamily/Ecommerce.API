using System.ComponentModel.DataAnnotations;

namespace Ecommerce.API.DTOs.Coupons
{
	public class CreateCouponDto
	{
		[Required]
		public string Name { get; set; }

		[Required]
		public decimal MinAmount { get; set; }

		[Required]
		public decimal MaxAmount { get; set; }

		public string Category { get; set; }

		[Required]
		public string ShopId { get; set; }

		[Required]
		public int Value { get; set; }
	}
}
