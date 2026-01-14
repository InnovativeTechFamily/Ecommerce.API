using System.ComponentModel.DataAnnotations;

namespace Ecommerce.API.Entities.Coupons
{
	public class Coupon
	{
		public string Id { get; set; } = NewId();   // "cu_abc"
		public static string NewId() => $"cu_{Guid.CreateVersion7()}";
		public string Name { get; set; }
		public decimal MinAmount { get; set; }
		public decimal MaxAmount { get; set; }
		public string Category { get; set; }
		[Required]
		public string ShopId { get; set; }
		public int Value { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}
