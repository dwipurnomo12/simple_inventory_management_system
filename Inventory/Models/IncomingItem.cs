using System.ComponentModel.DataAnnotations;

namespace Inventory.Models
{
    public class IncomingItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime DateOfEntry { get; set; }

        public string? TransactionCode { get; set; }

        [Required]
        public int StockIn { get; set; }

        [Required]
        public int ItemId { get; set; }
        public Item? Item { get; set; }

        [Required]
        public int SupplierId { get; set; }
        public Supplier? Supplier { get; set; }
    }
}
