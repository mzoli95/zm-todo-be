using System.ComponentModel.DataAnnotations;

namespace zm_todo.Validation
{
    public class DataCheckAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var date = (DateTime?)value;
            if(date < DateTime.Now)
            {
                return new ValidationResult("The date must be greated than or equal to todays date");
            }

            return ValidationResult.Success;
        }
    }
}
