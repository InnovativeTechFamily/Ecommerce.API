namespace Ecommerce.API.DTOs.User
{
	public class CreateUserAddressDto
	{
		public string AddressType { get; set; } = default!; // "Home", "Work".
		public string AddressLine1 { get; set; } = default!;
		public string? AddressLine2 { get; set; }
		public string City { get; set; }
		public string State { get; set; } 
        public string Country { get; set; }
		public string ZipCode { get; set; } 
	}
}
