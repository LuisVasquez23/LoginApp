using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "The {0} field is required")]
        [EmailAddress(ErrorMessage = "The field must be a valid email address")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "The {0} field is required")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
