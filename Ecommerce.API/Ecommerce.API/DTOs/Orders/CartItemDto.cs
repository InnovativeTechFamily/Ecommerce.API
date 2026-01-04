namespace Ecommerce.API.DTOs.Orders
{
    public class CartItemDto
    {
        public string Id { get; set; } = default!;  // _id
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Category { get; set; } = default!;
        public string Tags { get; set; } = default!;
        public decimal OriginalPrice { get; set; }
        public decimal DiscountPrice { get; set; }
        public int Stock { get; set; }
        public string ShopId { get; set; } = default!;
        public int Qty { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
    }

}
