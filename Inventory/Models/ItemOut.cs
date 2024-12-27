using System.ComponentModel.DataAnnotations;

namespace Inventory.Models
{
    public class ItemOut
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateOnly DateOfEntry { get; set; }

        public String? TransactionCode { get; set; }

        [Required]
        public int StockOut {  get; set; }

        [Required]
        public int ItemId { get; set; } 
        public Item? Item { get; set; }

        [Required]
        public int CustomerId   { get; set; }
        public Customer? Customer { get; set; }
    }
}
