using System.ComponentModel.DataAnnotations;

namespace Inventory.Models
{
    public class Supplier
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string SupplierName { get; set; }
        [Required]
        public string SupplierLocation { get; set; }
    }
}
