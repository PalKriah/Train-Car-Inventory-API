using System.ComponentModel.DataAnnotations;

namespace Train_Car_Inventory_App.Models.Payloads
{
    public class LoginRequest
    {
        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }
    }
}