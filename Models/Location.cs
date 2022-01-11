using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Train_Car_Inventory_App.Models
{
    public class Location : AbstractEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Owner { get; set; }
        
        [StringLength(500)]
        public string PostalAddress { get; set; }
        
        [Required]
        public int CodeNumber { get; set; }
        
        [JsonIgnore]
        public bool IsDeleted { get; set; }
    }
}