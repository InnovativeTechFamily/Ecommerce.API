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
	}
}
