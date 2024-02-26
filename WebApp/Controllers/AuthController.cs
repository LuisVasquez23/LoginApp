using Domain.Models;
using Domain.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using Application.Services.User;

namespace WebApp.Controllers
{
    [AllowAnonymous]
    public class AuthController :  BaseController<AuthController>
    {
        private IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        [AllowAnonymous]
        public IActionResult Login(string mensaje = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (mensaje is not null)
            {
                ViewData["mensaje"] = mensaje;
            }

            return View();
        }


        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegistroViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingUser = await _authService.FindUserByEmail(model);

            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "El correo electrónico ya está en uso.");
                return View(model);
            }

            AuthServiceViewModel result = await _authService.RegisterUser(model);

            if (result.errores != null )
            {
                foreach (var error in result.errores)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
                
            }


            UserModel? user = result.user;

            SetSession(user.Id, user.UserName, user.FullName);
            return RedirectToAction("Index", "Home");

        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {

            await _authService.LogOut();
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Auth");
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _authService.FindUserByEmail(model);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email is not registered");
                return View(model);
            }

            var result = await _authService.LoginUser(model , user);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Email or password is incorrect");
                return View(model);
            }


            SetSession(user.Id, user.UserName, user.FullName);
            return RedirectToAction("Index", "Home");

        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ChallengeResult> LoginExterno(string proveedor, string urlRetorno = null)
        {
            var urlRedireccion = Url.Action("RegistrarUsuarioExterno", values: new { urlRetorno });
            var properties = await _authService.LogInExternal(proveedor , urlRedireccion);
            return new ChallengeResult(proveedor, properties);

        }

        [AllowAnonymous]
        public async Task<IActionResult> RegistrarUsuarioExterno(string urlRetorno = null, string remoteError = null)
        {
            urlRetorno = urlRetorno ?? Url.Content("~");
            urlRetorno = urlRetorno.Equals("") ? "/Home/Index" : "/User/Login";

            var mensaje = "";

            if (remoteError is not null)
            {
                mensaje = $"Error del proveedor externo: {remoteError}";
                return RedirectToAction("Login", routeValues: new { mensaje });
            }

            var info = await _authService.ExternalLoginInfo();

            if (info is null)
            {
                mensaje = $"Error cargando la data de login externo";
                return RedirectToAction("Login", routeValues: new { mensaje });
            }

            var result = await _authService.LoginUser(info);

            if (result.Succeeded)
            {
                return LocalRedirect(urlRetorno);
            }

            string? email = "";
            string? userName = "";

            if (!info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                mensaje = $"Error leyendo email del usuario";
                return RedirectToAction("Login", routeValues: new { mensaje });
            }

            email = info.Principal.FindFirstValue(ClaimTypes.Email) ?? "";
            userName = info.Principal.FindFirstValue(ClaimTypes.Name);

            string emailAdd = info.Principal.FindFirstValue(ClaimTypes.Email) ?? "";
            string userNameAdd = info.Principal.FindFirstValue(ClaimTypes.Name);
            string validUserName = RemoveAccentsAndSpecialCharacters(userName);

            var user = new UserModel { Email = emailAdd, UserName = validUserName, FullName = userNameAdd };

            var resultCreateUser = await _authService.RegisterUser(user);

            if (!resultCreateUser.Succeeded)
            {
                mensaje = resultCreateUser.Errors.First().Description;
                return RedirectToAction("Login", routeValues: new { mensaje });
            }

            var resultAdd = await _authService.LoginUser(user, info);

            if (resultAdd.Succeeded)
            {
                SetSession(user.Id , user.UserName , user.FullName);
                await _authService.AddSingInUser(user, info);
                return LocalRedirect(urlRetorno);
            }

            mensaje = "Ha ocurrido un error agregando el login";
            return RedirectToAction("Login", routeValues: new { mensaje });
        }

        private string RemoveAccentsAndSpecialCharacters(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            input = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in input)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark && char.IsLetter(c))
                    sb.Append(c);
            }
            return sb.ToString();
        }

        private void SetSession(string idUser, string nombreUsuario , string fullName = "anonimo")
        {
            HttpContext.Session.SetString("ID_USUARIO", idUser);
            HttpContext.Session.SetString("NOMBRE_USUARIO", nombreUsuario);
            HttpContext.Session.SetString("FULL_NAME", fullName);
        }
    }
}
