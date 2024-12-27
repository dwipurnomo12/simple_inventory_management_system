using System.ComponentModel.DataAnnotations;

namespace Inventory.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string CustomerName { get; set; }

        [Required]
        public string CustomerLocation { get; set; }
    }
}
