using Domain.Models;
using Domain.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Application.Services.User
{
    public interface IAuthService
    {
        Task<UserModel> FindUserByEmail(RegistroViewModel model);
        Task<UserModel> FindUserByEmail(LoginViewModel model);
        Task<AuthServiceViewModel> RegisterUser(RegistroViewModel model);
        Task<IdentityResult> RegisterUser(UserModel model);
        void SingInUser(UserModel user);
        Task LogOut();
        Task<SignInResult> LoginUser(LoginViewModel model, UserModel user);
        Task<AuthenticationProperties> LogInExternal(string proveedor, string urlRedireccion);
        Task<ExternalLoginInfo> ExternalLoginInfo();
        Task<SignInResult> LoginUser(ExternalLoginInfo info);
        Task<IdentityResult> LoginUser(UserModel user, ExternalLoginInfo info);
        Task AddSingInUser(UserModel user, ExternalLoginInfo info);
    }
}
