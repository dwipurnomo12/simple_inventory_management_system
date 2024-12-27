using System.ComponentModel.DataAnnotations;

namespace Inventory.Models
{
    public class Unit
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UnitName { get; set; }
    }
}
