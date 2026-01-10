using Ecommerce.API.Data;
using Ecommerce.API.DTOs.Coupons;
using Ecommerce.API.Entities.Coupons;
using Ecommerce.API.Utils;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Services
{
	public class CouponService : ICouponService
	{
		private readonly ApplicationDbContext _context;

		public CouponService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<bool> CreateAsync(CreateCouponDto dto)
		{
			var exists = await _context.Coupons
				.AnyAsync(x => x.Name == dto.Name && x.ShopId == dto.ShopId);

			if (exists)
			{
				throw new ErrorHandler("Coupon already exists", 400);
			}

			var coupon = new Coupon
			{
				Name = dto.Name,
				MinAmount = dto.MinAmount,
				MaxAmount = dto.MaxAmount,
				Category = dto.Category,
				ShopId = dto.ShopId,
				Value = dto.Value
			};

			await _context.Coupons.AddAsync(coupon);
			await _context.SaveChangesAsync();

			return true;
		}
	}
}
