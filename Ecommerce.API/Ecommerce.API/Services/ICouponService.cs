using Ecommerce.API.DTOs.Coupons;
using Ecommerce.API.Entities.Coupons;

namespace Ecommerce.API.Services
{
	public interface ICouponService
	{
		Task<bool> CreateAsync(CreateCouponDto dto);
		Task<List<Coupon>> GetByShopIdAsync(string shopId);
		Task<bool> DeleteAsync(string id);
		Task<Coupon?> GetByNameAsync(string name);


	}
}
