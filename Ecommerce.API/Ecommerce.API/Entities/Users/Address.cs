using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.API.Entities.Users
{
    public class Address
    {
        [Key]
        public Guid Id { get; set; }

        public string? Country { get; set; }

        public string? City { get; set; }

        [StringLength(200)]
        public string? Address1 { get; set; }

        [StringLength(200)]
        public string? Address2 { get; set; }

        public string? ZipCode { get; set; }

        [StringLength(50)]
        public string? AddressType { get; set; }

        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
