using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using PracticaPreexamenCubos.Models;
using PracticaPreexamenCubos.Repositories;

namespace PracticaPreexamenCubos.Controllers
{
    public class ManagedController : Controller
    {
        private RepositoryCubos repo;

        public ManagedController(RepositoryCubos repo)
        {
            this.repo = repo;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            Usuario usuario = await this.repo.LoginUsuarioAsync(email, password);

            if (usuario != null)
            {
                ClaimsIdentity identity = new ClaimsIdentity (CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                Claim claimID = new Claim(ClaimTypes.NameIdentifier, usuario.Id_user.ToString());
                Claim claimEmail = new Claim(ClaimTypes.Name, usuario.Email);
                Claim claimNombre = new Claim("NOMBRE", usuario.Nombre);
               
                identity.AddClaim(claimID);
                identity.AddClaim(claimEmail);
                identity.AddClaim(claimNombre);

                ClaimsPrincipal userPrincipal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);
                string controller = TempData["controller"]?.ToString() ?? "Cubos";
                string action = TempData["action"]?.ToString() ?? "Index";

                return RedirectToAction(action, controller);
            }
            else
            {
                ViewData["MENSAJE"] = "Email/password incorrectos";
                return View();
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Managed");
        }
    }
}
