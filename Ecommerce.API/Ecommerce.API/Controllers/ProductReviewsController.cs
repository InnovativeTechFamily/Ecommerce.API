using Ecommerce.API.DTOs.Products;
using Ecommerce.API.Entities.Users;
using Ecommerce.API.Middleware;
using Ecommerce.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{
	[Route("api/reviews")]
	[ApiController]
	public class ProductReviewsController : ControllerBase
	{
		private readonly IProductReviewService _reviewService;

		public ProductReviewsController(IProductReviewService reviewService)
		{
			_reviewService = reviewService;
		}

		[HttpPost("create")]
		[IsAuthenticated]
		public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto dto)
		{
			var user = HttpContext.Items["User"] as User;

			if (user == null)
				return Unauthorized();

			await _reviewService.AddOrUpdateReviewAsync(user.Id, dto);

			return Ok(new
			{
				success = true,
				message = "Reviewed successfully"
			});
		}
	}
}
