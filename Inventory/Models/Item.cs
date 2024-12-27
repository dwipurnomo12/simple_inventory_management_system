using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Models
{
    public class Item
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string ItemName { get; set; }

        public String? ItemImage { get; set; }   

        [Required]
        [StringLength(1000)]
        public string ItemDescription { get; set; }

        [Range(0, int.MaxValue)]
        public int? ItemStock { get; set; } = 0;

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public int UnitId { get; set; }
        public Unit? Unit { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; }
    }
}
