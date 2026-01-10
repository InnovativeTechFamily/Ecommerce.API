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
		public async Task<List<Coupon>> GetByShopIdAsync(string shopId)
		{
			var coupons = await _context.Coupons
				.Where(x => x.ShopId == shopId)
				.OrderByDescending(x => x.CreatedAt)
				.ToListAsync();

			return coupons;
		}
		public async Task<bool> DeleteAsync(string id)
		{
			// Find coupon
			var coupon = await _context.Coupons.FirstOrDefaultAsync(x => x.Id == id);

			if (coupon == null)
				return false;

			// Remove coupon
			_context.Coupons.Remove(coupon);

			// Save changes
			await _context.SaveChangesAsync();

			return true;
		}

		public async Task<Coupon?> GetByNameAsync(string name)
		{
			return await _context.Coupons
				.FirstOrDefaultAsync(x => x.Name == name);
		}

	}
}
