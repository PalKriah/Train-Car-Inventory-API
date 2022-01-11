using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Train_Car_Inventory_App.Models.Validation
{
    public class TrackNumberValidator : ValidationAttribute
    {
        public string GetErrorMessage() => "Track number checksum is incorrect";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var trackNumber = ((string) value).Replace(" ", "").Replace("-", "");

            var last = Convert.ToInt32(trackNumber.Last().ToString());
            var sum = 0;
            var multiplier = 2;

            for (int i = 0; i < trackNumber.Length - 1; i++)
            {
                sum += multiplier * Convert.ToInt32(trackNumber[i].ToString()) % 10;
                multiplier = multiplier == 2 ? 1 : 2;
            }

            var expectedLast = (10 - sum % 10) % 10;

            return expectedLast != last ? new ValidationResult(GetErrorMessage()) : ValidationResult.Success;
        }
    }
}