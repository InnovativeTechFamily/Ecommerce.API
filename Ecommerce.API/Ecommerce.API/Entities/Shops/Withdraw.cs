using System.ComponentModel.DataAnnotations;

namespace Ecommerce.API.Entities.Shops
{
    public class Withdraw
    {
        [Key]
        public string Id { get; set; } = NewId();
        public static string NewId() => $"w_{Guid.CreateVersion7()}";

        [Required]
        public WithdrawSeller Seller { get; set; } = default!;

        [Required]
        public decimal Amount { get; set; }

        public string Status { get; set; } = "Processing";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    public class WithdrawSeller
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
    }
}
