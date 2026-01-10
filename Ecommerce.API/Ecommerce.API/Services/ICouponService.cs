using Ecommerce.API.DTOs.Coupons;

namespace Ecommerce.API.Services
{
	public interface ICouponService
	{
		Task<bool> CreateAsync(CreateCouponDto dto);
	}
}
