using Ecommerce.API.Data;
using Ecommerce.API.DTOs.Orders;
using Ecommerce.API.Entities.Orders;
using Ecommerce.API.Utils;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Ecommerce.API.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _db;
        private readonly IOrderEmailService _emailService;

        public OrderService(ApplicationDbContext db, IOrderEmailService emailService)
        {
            _db = db;
            _emailService = emailService;
        }

        public async Task<IReadOnlyList<Order>> CreateOrdersAsync(CreateOrderRequestDto dto)
        {
            // group cart items by shopId
            var groups = dto.Cart.GroupBy(c => c.ShopId);

            var orders = new List<Order>();

            foreach (var group in groups)
            {
                var items = group.Select(i => new OrderItem
                {
                    ProductId = i.Id,
                    Name = i.Name,
                    OriginalPrice = i.OriginalPrice,
                    DiscountPrice = i.DiscountPrice,
                    Qty = i.Qty,
                    Size = i.Size,
                    Color = i.Color,
                    ShopId = i.ShopId,
                    ShopName = "" // optional: fill from product or shop table if needed
                }).ToList();

                var order = new Order
                {
                    Cart = items,
                    ShippingAddress = new ShippingAddress
                    {
                        Address1 = dto.ShippingAddress.Address1,
                        Address2 = dto.ShippingAddress.Address2,
                        City = dto.ShippingAddress.City,
                        Country = dto.ShippingAddress.Country,
                        ZipCode = dto.ShippingAddress.ZipCode
                    },
                    User = new OrderUser
                    {
                        UserId = dto.User.Id,
                        Name = dto.User.Name,
                        Email = dto.User.Email,
                        Addresses = dto.User.Addresses.Select(a => new OrderUserAddress
                        {
                            Address1 = a.Address1,
                            Address2 = a.Address2,
                            City = a.City,
                            Country = a.Country,
                            ZipCode = a.ZipCode,
                            AddressType = a.AddressType
                        }).ToList()
                    },
                    TotalPrice = dto.TotalPrice,
                    PaymentInfo = new PaymentInfo
                    {
                        Id = dto.PaymentInfo.Id,
                        Status = dto.PaymentInfo.Status,
                        Type = dto.PaymentInfo.Type
                    }
                };

                orders.Add(order);
                _db.Orders.Add(order);
            }

            await _db.SaveChangesAsync();

            // email based on first order (same as Node)
            var firstOrder = orders.First();

            var orderId = firstOrder.Id;
            var subTotalPrice = firstOrder.Cart.Sum(i => i.Qty * i.DiscountPrice);
            var shipping = subTotalPrice * 0.1m;

            var orderItemsHtml = new StringBuilder();
            foreach (var item in firstOrder.Cart)
            {
                orderItemsHtml.AppendLine($@"
<tr>
    <td>{item.Name}</td>
    <td>x{item.Qty}</td>
    <td>{(string.IsNullOrEmpty(item.Size) ? "-" : item.Size)}</td>
    <td>{(string.IsNullOrEmpty(item.Color) ? "-" : item.Color)}</td>
    <td><del>${item.OriginalPrice}</del> ${item.DiscountPrice}</td>
</tr>");
            }

            var paymentDetailsHtml = $@"
<tr>
    <td colspan=""4""><strong>Shipping Charge: ${shipping:F2}</strong></td>
</tr>
<tr>
    <td colspan=""4""><strong>Total Payment: ${dto.TotalPrice}</strong></td>
</tr>
<tr>
    <td colspan=""3"">Payment Type: {dto.PaymentInfo.Type}</td>
    <td>Status: {(string.IsNullOrEmpty(dto.PaymentInfo.Status) ? "-" : dto.PaymentInfo.Status)}</td>
</tr>";

            var billing = dto.User.Addresses.FirstOrDefault();
            var billingAddressHtml = $@"
<tr>
    <td><strong>Billing Address</strong></td>
    <td><strong>Shipping Address</strong></td>
</tr>
<tr>
    <td>{billing?.Address1}, {billing?.Address2}, {billing?.ZipCode}</td>
    <td>{dto.ShippingAddress.Address1}, {dto.ShippingAddress.Address2}, {dto.ShippingAddress.ZipCode}</td>
</tr>";

            var subject = "Your E-Shop order has been received!";
            var emailHtml = $@"
<p>Hello {dto.User.Name},</p>
<p>Just to let you know — we've received your order #{orderId}, and it is now being processed:</p>
<strong>Order Summary:</strong>
<table border=""1"" cellpadding=""5"">
    <tr>
        <td>Name</td>
        <td>QTY</td>
        <td>Size</td>
        <td>Color</td>
        <td>Price</td>
    </tr>
    {orderItemsHtml}
    {paymentDetailsHtml}
    {billingAddressHtml}
</table>";

            await _emailService.SendOrderConfirmationAsync(dto.User.Email, subject, emailHtml);

            return orders;
        }

        public async Task<IReadOnlyList<Order>> GetOrdersByUserIdAsync(string userId)
        {
            return await _db.Orders
                .Where(o => o.User.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }
        public async Task<IReadOnlyList<Order>> GetOrdersByShopIdAsync(string shopId)
        {
            return await _db.Orders
                .Where(o => o.Cart.Any(item => item.ShopId == shopId))
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }
        public async Task<Order> UpdateOrderStatusAsync(string orderId, string status, string sellerId)
        {
            var order = await _db.Orders.FindAsync(orderId);
            if (order == null)
                throw new ErrorHandler("Order not found with this id", 400);

            // Update stock for all cart items if "Transferred to delivery partner"
            if (status == "Transferred to delivery partner")
            {
                foreach (var item in order.Cart)
                {
                    var product = await _db.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        product.Stock -= item.Qty;
                        product.SoldOut += item.Qty;
                        _db.Products.Update(product);
                    }
                }
            }

            // Update order status
            order.Status = status;

            if (status == "Delivered")
            {
                order.DeliveredAt = DateTime.UtcNow;
                order.PaymentInfo.Status = "Succeeded";

                // Update seller balance (10% service charge)
                var serviceCharge = order.TotalPrice * 0.1m;
                var sellerAmount = order.TotalPrice - serviceCharge;

                var seller = await _db.Shops.FindAsync(sellerId);
                if (seller != null)
                {
                    seller.AvailableBalance += sellerAmount;
                    _db.Shops.Update(seller);
                }
            }

            await _db.SaveChangesAsync();

            return order;
        }

        public async Task<Order> RequestOrderRefundAsync(string orderId, string status)
        {
            var order = await _db.Orders.FindAsync(orderId);
            if (order == null)
                throw new ErrorHandler("Order not found with this id", 400);

            order.Status = status;

            await _db.SaveChangesAsync();

            return order;
        }
        public async Task<Order> AcceptOrderRefundAsync(string orderId, string status, string sellerId)
        {
            var order = await _db.Orders.FindAsync(orderId);
            if (order == null)
                throw new ErrorHandler("Order not found with this id", 400);

            order.Status = status;

            // Restore product stock if "Refund Success"
            if (status == "Refund Success")
            {
                foreach (var item in order.Cart)
                {
                    var product = await _db.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        product.Stock += item.Qty;
                        product.SoldOut -= item.Qty;
                        _db.Products.Update(product);
                    }
                }
            }

            await _db.SaveChangesAsync();

            return order;
        }

        public async Task<IReadOnlyList<Order>> GetAllOrdersAdminAsync()
        {
            return await _db.Orders
                .OrderByDescending(o => o.DeliveredAt)
                .ThenByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

    }

}
