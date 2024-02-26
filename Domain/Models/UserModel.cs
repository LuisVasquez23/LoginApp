using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class UserModel : IdentityUser
    {

        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "El campo {0} solo puede contener letras, dígitos o espacios")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string? FullName { get; set; }
        
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
        public override string? Email { get; set; }
    }
}
