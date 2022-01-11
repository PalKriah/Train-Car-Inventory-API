using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Train_Car_Inventory_App.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        [Required]
        public DateTime BirthDate { get; set; }
        
        public bool IsRailwayWorker { get; set; }
        
        [StringLength(100)]
        public string RailwayCompany { get; set; }

        public virtual ICollection<IdentityUserRole<int>> Roles { get; } = new List<IdentityUserRole<int>>();
    }
}