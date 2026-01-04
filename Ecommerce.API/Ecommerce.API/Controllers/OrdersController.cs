using Ecommerce.API.DTOs.Orders;
using Ecommerce.API.Entities.Shops;
using Ecommerce.API.Middleware;
using Ecommerce.API.Services;
using Ecommerce.API.Utils;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost("create-order")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto dto)
    {
        try
        {
            if (dto.Cart == null || dto.Cart.Count == 0)
            {
                throw new ErrorHandler("Cart is required", 400);
            }

            var orders = await _orderService.CreateOrdersAsync(dto);

            return StatusCode(StatusCodes.Status201Created, new
            {
                success = true,
                orders
            });
        }
        catch (ErrorHandler)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ErrorHandler(ex.Message, 500);
        }
    }

    [HttpGet("get-all-orders/{userId}")]
    public async Task<IActionResult> GetAllOrders(string userId)
    {
        try
        {
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(new { success = true, orders });
        }
        catch (Exception ex)
        {
            throw new ErrorHandler(ex.Message, 500);
        }
    }
    [HttpGet("get-seller-all-orders/{shopId}")]
    public async Task<IActionResult> GetSellerAllOrders(string shopId)
    {
        try
        {
            var orders = await _orderService.GetOrdersByShopIdAsync(shopId);
            return Ok(new { success = true, orders });
        }
        catch (Exception ex)
        {
            throw new ErrorHandler(ex.Message, 500);
        }
    }
    [HttpPut("update-order-status/{id}")]
    [IsSeller]
    public async Task<IActionResult> UpdateOrderStatus(string id, [FromBody] UpdateOrderStatusDto dto)
    {
        try
        {
            // sellerId from IsSellerAttribute (like req.seller.id)
            var sellerId = (HttpContext.Items["Seller"] as Shop)?.Id;
            if (sellerId == null)
                throw new ErrorHandler("Seller not authenticated", 401);

            var order = await _orderService.UpdateOrderStatusAsync(id, dto.Status, sellerId.ToString());
            return Ok(new { success = true, order });
        }
        catch (ErrorHandler)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ErrorHandler(ex.Message, 500);
        }
    }

    [HttpPut("order-refund/{id}")]
    public async Task<IActionResult> OrderRefund(string id, [FromBody] UpdateOrderStatusDto dto)
    {
        try
        {
            var order = await _orderService.RequestOrderRefundAsync(id, dto.Status);
            return Ok(new
            {
                success = true,
                order,
                message = "Order Refund Request successfully!"
            });
        }
        catch (ErrorHandler)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ErrorHandler(ex.Message, 500);
        }
    }

    [HttpPut("order-refund-success/{id}")]
    [IsSeller]
    public async Task<IActionResult> OrderRefundSuccess(string id, [FromBody] UpdateOrderStatusDto dto)
    {
        try
        {
            // sellerId from IsSellerAttribute
            var sellerId = (HttpContext.Items["Seller"] as Shop)?.Id;
            if (sellerId == null)
                throw new ErrorHandler("Seller not authenticated", 401);

            await _orderService.AcceptOrderRefundAsync(id, dto.Status, sellerId.ToString());

            return Ok(new
            {
                success = true,
                message = "Order Refund successfull!"
            });
        }
        catch (ErrorHandler)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ErrorHandler(ex.Message, 500);
        }
    }
    [HttpGet("admin-all-orders")]
    [IsAuthenticated]
    [IsAdmin("Admin")]
    public async Task<IActionResult> AdminAllOrders()
    {
        try
        {
            var orders = await _orderService.GetAllOrdersAdminAsync();
            return StatusCode(StatusCodes.Status201Created, new
            {
                success = true,
                orders
            });
        }
        catch (Exception ex)
        {
            throw new ErrorHandler(ex.Message, 500);
        }
    }


}
