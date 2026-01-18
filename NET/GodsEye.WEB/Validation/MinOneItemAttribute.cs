using System.ComponentModel.DataAnnotations;
using System.Collections;

namespace GodsEye.WEB.Validation
{
    public class MinOneItemAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if (value is IList list && list.Count > 0)
                return ValidationResult.Success;

            return new ValidationResult("Selecione uma ou mais funcionalidade(s).");
        }
    }
}
