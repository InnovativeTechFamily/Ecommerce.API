using Ecommerce.API.Data;
using Ecommerce.API.Entities.Shops;
using Ecommerce.API.Utils;

namespace Ecommerce.API.Services
{
    public class WithdrawService : IWithdrawService
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailService _emailService;

        public WithdrawService(ApplicationDbContext db, IEmailService emailService)
        {
            _db = db;
            _emailService = emailService;
        }

        public async Task<Withdraw> CreateWithdrawRequestAsync(string sellerId, decimal amount)
        {
            // Load seller info
            var shopId = Guid.Parse(sellerId);
            var seller = await _db.Shops.FindAsync(shopId);
            if (seller == null)
                throw new ErrorHandler("Seller not found", 404);

            if (seller.AvailableBalance < amount)
                throw new ErrorHandler("Insufficient balance", 400);

            // Send email first (like Node.js)
            try
            {
                await _emailService.SendWithdrawRequestEmailAsync(
                    seller.Email,
                    seller.Name,
                    amount
                );
            }
            catch (Exception ex)
            {
                throw new ErrorHandler($"Email failed: {ex.Message}", 500);
            }

            // Create withdraw request
            var withdraw = new Withdraw
            {
                Seller = new WithdrawSeller
                {
                    Id = seller.Id.ToString(),
                    Name = seller.Name,
                    Email = seller.Email
                },
                Amount = amount
            };

            _db.Withdraws.Add(withdraw);

            // Deduct from seller balance (like Node.js)
            seller.AvailableBalance -= amount;
            _db.Shops.Update(seller);

            await _db.SaveChangesAsync();

            return withdraw;
        }
    }
}
