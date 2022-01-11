using System.ComponentModel.DataAnnotations;

namespace Train_Car_Inventory_App.Models
{
    public abstract class AbstractEntity
    {
        [Required]
        [Key]
        public int Id { get; set; }
    }
}