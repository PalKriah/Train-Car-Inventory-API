using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Train_Car_Inventory_App.Models.Validation;

namespace Train_Car_Inventory_App.Models
{
    public class TrainCar : AbstractEntity
    {
        [Required]
        [StringLength(10)]
        public string SerialNumber { get; set; }

        [Required]
        [Range(0, 9999)]
        public int ManufacturingYear { get; set; }
        
        [Required]
        [MinLength(17), MaxLength(17)]
        [TrackNumberValidator]
        public string TrackNumber { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Owner { get; set; }
        
        public Location Location { get; set; }
        [ForeignKey("Location")]
        public int LocationId { get; set; }
        
        [JsonIgnore]
        public bool IsDeleted { get; set; }
        
        public DateTime ScrapDate { get; set; }
    }
}