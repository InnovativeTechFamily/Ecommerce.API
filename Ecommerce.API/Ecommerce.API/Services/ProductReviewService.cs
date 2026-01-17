using Ecommerce.API.Data;
using Ecommerce.API.DTOs.Products;
using Ecommerce.API.Entities.Products;
using Ecommerce.API.Utils;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Services
{
    public class ProductReviewService : IProductReviewService
    {
        private readonly ApplicationDbContext _context;

        public ProductReviewService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddOrUpdateReviewAsync(int userId, CreateReviewDto dto)
        {
            var product = await _context.Products
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == dto.ProductId);

            if (product == null)
                throw new ErrorHandler("Product not found", 404);

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == dto.OrderId);

            if (order == null)
                throw new ErrorHandler("Order not found", 404);

            var orderItem = order.Cart
                .FirstOrDefault(c => c.ProductId == dto.ProductId);

            if (orderItem == null)
                throw new ErrorHandler("Product not found in order", 400);

            if (orderItem.IsReviewed)
                throw new ErrorHandler("Product already reviewed", 400);

            var existingReview = product.Reviews
                .FirstOrDefault(r => r.UserId == userId);

            if (existingReview != null)
            {
                existingReview.Rating = dto.Rating;
                existingReview.Comment = dto.Comment;
            }
            else
            {
                product.Reviews.Add(new ProductReview
                {
                    ProductId = dto.ProductId,
                    UserId = userId,
                    Rating = dto.Rating,
                    Comment = dto.Comment
                });
            }

            product.Ratings = product.Reviews.Average(r => r.Rating);
            orderItem.IsReviewed = true;

            await _context.SaveChangesAsync();
        }
    }
}
