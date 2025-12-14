using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.API.Entities.Users
{
    public class Avatar
    {
        [Key]
        public Guid Id { get; set; }

        public string PublicId { get; set; }

        public string Url { get; set; }

        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
