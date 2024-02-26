using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModels
{
    public class RegistroViewModel
    {
        [Required(ErrorMessage = "The {0} field is required")]
        [EmailAddress(ErrorMessage = "The field must be a valid email address")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "The {0} field is required")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "The entered FullName is invalid. It should only contain letters")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string? FullName { get; set; }
    }
}
