namespace Ecommerce.API.Entities
{
	public class Category
	{
		public string Id { get; set; } = NewId();   // "c_abc"
		public static string NewId() => $"c_{Guid.CreateVersion7()}";
		public string Name { get; set; }
		public string Code { get; set; }
	}
}
