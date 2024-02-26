using Domain.Models;
using Domain.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Application.Services.User
{
    public class AuthService : IAuthService
    {

        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;

        public AuthService(UserManager<UserModel> userManager, SignInManager<UserModel> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<UserModel> FindUserByEmail(RegistroViewModel model)
        {
            UserModel? user = await _userManager.FindByEmailAsync(model.Email);

            return user;
        }

        public async Task<AuthServiceViewModel> RegisterUser(RegistroViewModel model)
        {
            List<IdentityError> errores = new List<IdentityError>();

            var user = new UserModel
            {
                Email = model.Email,
                UserName = model?.FullName?.Replace(" ", "_"),
                FullName = model?.FullName,
            };

            var result = await _userManager.CreateAsync(user, password: model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    errores.Add(error);
                }

                return new AuthServiceViewModel { user = user, errores = errores };
                
            }

            return new AuthServiceViewModel { user = user, errores = null };


        }

        public async Task<IdentityResult> RegisterUser(UserModel model)
        {
            return await _userManager.CreateAsync(model);

        }

        public async void SingInUser(UserModel user)
        {
            await _signInManager.SignInAsync(user, isPersistent: true);
        } 

        public async Task LogOut()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<UserModel> FindUserByEmail(LoginViewModel model)
        {
            UserModel? user = await _userManager.FindByEmailAsync(model.Email);

            return user;
        }

        public async Task<SignInResult> LoginUser(LoginViewModel model , UserModel user)
        {
            return await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);
        }

        public async Task<AuthenticationProperties> LogInExternal(string proveedor, string urlRedireccion)
        {
           return  _signInManager.ConfigureExternalAuthenticationProperties(proveedor, urlRedireccion);
        }

        public async Task<ExternalLoginInfo> ExternalLoginInfo()
        {
            return await _signInManager.GetExternalLoginInfoAsync();
        }

        public async Task<SignInResult> LoginUser(ExternalLoginInfo info)
        {
           return await _signInManager
                    .ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true, bypassTwoFactor: true);
        }

        public async Task<IdentityResult> LoginUser(UserModel user , ExternalLoginInfo info)
        {
            return await _userManager.AddLoginAsync(user, info);
        }

        public async Task AddSingInUser(UserModel user, ExternalLoginInfo info)
        {
            await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
        }

    }
}
