using Ecommerce.API.DTOs.Coupons;
using Ecommerce.API.Services;
using Ecommerce.API.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{
	[ApiController]
	[Route("api/coupons")]
	public class CouponController : ControllerBase
	{
		private readonly ICouponService _couponService;

		public CouponController(ICouponService couponService)
		{
			_couponService = couponService;
		}

		[HttpPost("create")]
		public async Task<IActionResult> CreateCoupon([FromBody] CreateCouponDto dto)
		{
			try
			{
				await _couponService.CreateAsync(dto);

				return Ok(new
				{
					success = true,
					message = "Coupon created successfully"
				});
			}
			catch (Exception ex)
			{
				throw new ErrorHandler(ex.Message, 500);
			}
		}
		[HttpGet("shop/{shopId}")]
		public async Task<IActionResult> GetCouponsByShop(string shopId)
		{
			try
			{
				var coupons = await _couponService.GetByShopIdAsync(shopId);

				return Ok(new
				{
					success = true,
					count = coupons.Count,
					data = coupons
				});
			}
			catch (Exception ex)
			{
				throw new ErrorHandler(ex.Message, 500);
			}
		}
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteCoupon(string id)
		{
			try
			{
				var deleted = await _couponService.DeleteAsync(id);

				if (!deleted)
				{
					throw new ErrorHandler("Coupon not found", 404);
				}

				return Ok(new
				{
					success = true,
					message = "Coupon deleted successfully"
				});
			}
			catch (Exception ex)
			{
				throw new ErrorHandler(ex.Message, 500);
			}
		}

		[HttpGet("get-coupon-value/{name}")]
		public async Task<IActionResult> GetCouponValue(string name)
		{
			try
			{
				var coupon = await _couponService.GetByNameAsync(name);

				if (coupon == null)
				{
					throw new ErrorHandler("Coupon not found", 404);
				}

				return Ok(new
				{
					success = true,
					value = coupon.Value,
					minAmount = coupon.MinAmount,
					maxAmount = coupon.MaxAmount
				});
			}
			catch (Exception ex)
			{
				throw new ErrorHandler(ex.Message, 500);
			}
		}


	}
}
	
