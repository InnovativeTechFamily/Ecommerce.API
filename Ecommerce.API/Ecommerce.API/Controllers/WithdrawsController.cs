using Ecommerce.API.DTOs.Shop;
using Ecommerce.API.Entities.Shops;
using Ecommerce.API.Middleware;
using Ecommerce.API.Services;
using Ecommerce.API.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WithdrawsController : ControllerBase
    {
        private readonly IWithdrawService _withdrawService;

        public WithdrawsController(IWithdrawService withdrawService)
        {
            _withdrawService = withdrawService;
        }

        [HttpPost("create-withdraw-request")]
        [IsSeller]
        public async Task<IActionResult> CreateWithdrawRequest([FromBody] CreateWithdrawRequestDto dto)
        {
            try
            {
                // sellerId from IsSellerAttribute
                var sellerId = (HttpContext.Items["Seller"] as Shop)?.Id;
                if (sellerId == null)
                    throw new ErrorHandler("Seller not authenticated", 401);

                var withdraw = await _withdrawService.CreateWithdrawRequestAsync(sellerId.ToString(), dto.Amount);
                return StatusCode(StatusCodes.Status201Created, new
                {
                    success = true,
                    withdraw
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
        [HttpGet("get-all-withdraw-request")]
        [IsAuthenticated]
        [IsAdmin("Admin")]
        public async Task<IActionResult> GetAllWithdrawRequests()
        {
            try
            {
                var withdraws = await _withdrawService.GetAllWithdrawRequestsAsync();
                return StatusCode(StatusCodes.Status201Created, new
                {
                    success = true,
                    withdraws
                });
            }
            catch (Exception ex)
            {
                throw new ErrorHandler(ex.Message, 500);
            }
        }

        [HttpPut("update-withdraw-request/{id}")]
        [IsAuthenticated]
        [IsAdmin("Admin")]
        public async Task<IActionResult> UpdateWithdrawRequest(string id, [FromBody] UpdateWithdrawRequestDto dto)
        {
            try
            {
                var withdraw = await _withdrawService.UpdateWithdrawRequestAsync(id, dto.SellerId);
                return StatusCode(StatusCodes.Status201Created, new
                {
                    success = true,
                    withdraw
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
    }
}
