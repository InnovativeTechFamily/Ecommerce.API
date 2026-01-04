using System.ComponentModel.DataAnnotations;

namespace Ecommerce.API.Entities.Orders
{

    public class Order
    {
        public string Id { get; set; } = NewId();
        public static string NewId() => $"o_{Guid.CreateVersion7()}";

        [Required]
        public List<OrderItem> Cart { get; set; } = new();

        [Required]
        public ShippingAddress ShippingAddress { get; set; } = default!;

        [Required]
        public OrderUser User { get; set; } = default!;

        [Required]
        public decimal TotalPrice { get; set; }

        public string Status { get; set; } = "Processing";

        public PaymentInfo PaymentInfo { get; set; } = new();

        public DateTime PaidAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeliveredAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class OrderItem
    {
        public string Id { get; set; } = NewId();
        public static string NewId() => $"oi_{Guid.CreateVersion7()}";

        public string ProductId { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Size { get; set; }
        public string? Color { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal DiscountPrice { get; set; }
        public int Qty { get; set; }

        public string ShopId { get; set; } = default!;
        public string ShopName { get; set; } = default!;
    }

    public class ShippingAddress
    {
        public string Address1 { get; set; } = default!;
        public string? Address2 { get; set; }
        public string City { get; set; } = default!;
        public string Country { get; set; } = default!;
        public int ZipCode { get; set; }
    }

    public class OrderUser
    {
        public string UserId { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;

        public List<OrderUserAddress> Addresses { get; set; } = new();
    }

    public class OrderUserAddress
    {
        public string Address1 { get; set; } = default!;
        public string? Address2 { get; set; }
        public string City { get; set; } = default!;
        public string Country { get; set; } = default!;
        public int ZipCode { get; set; }
        public string AddressType { get; set; } = default!;
    }

    public class PaymentInfo
    {
        public string? Id { get; set; }
        public string? Status { get; set; }
        public string? Type { get; set; }
    }

}
