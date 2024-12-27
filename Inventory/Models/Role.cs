using System.ComponentModel.DataAnnotations;

namespace Inventory.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string RoleName { get; set; }
    }
}
