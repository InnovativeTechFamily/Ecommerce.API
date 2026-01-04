namespace Ecommerce.API.DTOs.Orders
{
    public class OrderUserDto
    {
        public string Id { get; set; } = default!;   // _id
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Role { get; set; } = default!;
        public List<OrderUserAddressDto> Addresses { get; set; } = new();
    }

}
