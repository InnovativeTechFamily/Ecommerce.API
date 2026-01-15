using System.ComponentModel.DataAnnotations;

namespace Ecommerce.API.Entities.Shops
{
    // transaction sub-document
    public class ShopTransaction
    {
        public int Id { get; set; }          // PK for EF
        public string WithdrawId { get; set; } = default!; // FK to Withdraw
        public Guid ShopId { get; set; }     // FK to Shop
        public Shop Shop { get; set; } = default!;

        [Required]
        public decimal Amount { get; set; }

        public string Status { get; set; } = "Processing";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
