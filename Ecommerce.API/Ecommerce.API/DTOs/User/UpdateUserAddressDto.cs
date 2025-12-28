namespace Ecommerce.API.DTOs.User
{
    public class UpdateUserAddressDto
    {
        public int? Id { get; set; } // Optional: for updating existing address
        public string AddressType { get; set; } = default!; // "home", "work", etc.
        public string AddressLine1 { get; set; } = default!;
        public string? AddressLine2 { get; set; }
        public string City { get; set; } = default!;
        //public string State { get; set; } = default!;
        public string ZipCode { get; set; } = default!;
    }

}
