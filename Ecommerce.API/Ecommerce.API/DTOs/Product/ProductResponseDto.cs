using System;
using System.Collections.Generic;

namespace Ecommerce.API.DTOs.Products
{
	public class ProductResponseDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Category { get; set; }
		public string Tags { get; set; }
		public decimal? OriginalPrice { get; set; }
		public decimal DiscountPrice { get; set; }
		public int Stock { get; set; }
		public List<ProductImageResponseDto>? Images { get; set; }
		public decimal? Ratings { get; set; }
		public Guid ShopId { get; set; }
		public object? Shop { get; set; }
		public int SoldOut { get; set; }
		public int Status { get; set; }
		public bool IsPublished { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
	}

	public class ProductImageResponseDto
	{
		public string Id { get; set; }
		public string PublicId { get; set; }
		public string Url { get; set; }
	}
}