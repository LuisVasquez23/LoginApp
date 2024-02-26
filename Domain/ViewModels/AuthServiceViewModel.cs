using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Domain.ViewModels
{
    public class AuthServiceViewModel
    {
        public UserModel? user { get; set; }
        public List<IdentityError>? errores { get; set; }
    }
}
