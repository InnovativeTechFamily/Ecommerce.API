using Ecommerce.API.Entities.Shops;

namespace Ecommerce.API.Services
{
    public interface IWithdrawService
    {
        Task<Withdraw> CreateWithdrawRequestAsync(string sellerId, decimal amount);
    }
}
