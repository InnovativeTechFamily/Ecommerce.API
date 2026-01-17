using Ecommerce.API.DTOs.Products;

namespace Ecommerce.API.Services
{
	public interface IProductReviewService
	{
		Task AddOrUpdateReviewAsync(int userId, CreateReviewDto dto);
	}
}
