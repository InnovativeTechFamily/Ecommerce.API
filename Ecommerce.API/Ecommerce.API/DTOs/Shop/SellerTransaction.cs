namespace Ecommerce.API.DTOs.Shop
{
    public class SellerTransaction
    {
        public string Id { get; set; } = default!;
        public decimal Amount { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Status { get; set; } = default!;
    }

}
