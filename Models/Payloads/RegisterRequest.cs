using System;
using System.ComponentModel.DataAnnotations;

namespace Train_Car_Inventory_App.Models.Payloads
{
    public class RegisterRequest
    {
        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        [MinLength(8), StringLength(100)]
        public string Password { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }
        
        public bool IsRailwayWorker { get; set; }
        
        [StringLength(100)]
        public string RailwayCompany { get; set; }
    }
}