using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ExamenMvcLibrosGiovanny.Repositories;
using ExamenMvcLibrosGiovanny.Models;

namespace ExamenMvcLibrosGiovanny.Controllers {
    public class ManagedController : Controller {

        private RepositoryLibros repo;

        public ManagedController(RepositoryLibros repo) {
            this.repo = repo;
        }

        public IActionResult LogIn() {
            return View();
        }

        [HttpPost] [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogIn(string email, string password) {

            Usuario user = await this.repo.LoginUserAsync(email, password);

            if (user != null) { // El usuario existe en la BBDD
                ClaimsIdentity identity = new ClaimsIdentity(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        ClaimTypes.Name, ClaimTypes.Role
                );

                Claim claimUserID = new Claim("ID", user.IdUsuario.ToString());
                Claim claimUserName = new Claim(ClaimTypes.Name, user.Nombre);
                Claim claimUserLastName = new Claim("LAST_NAME", user.Apellidos);
                Claim claimUserEmail = new Claim("EMAIL", user.Email);
                Claim claimUserPhoto = new Claim("PHOTO", user.Foto);
                Claim claimUserRole = new Claim(ClaimTypes.Role, "cliente");

                identity.AddClaim(claimUserID);
                identity.AddClaim(claimUserName);
                identity.AddClaim(claimUserLastName);
                identity.AddClaim(claimUserEmail);
                identity.AddClaim(claimUserPhoto);
                identity.AddClaim(claimUserRole);

                ClaimsPrincipal userPrincipal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);

                string controller = TempData["controller"].ToString();
                string action = TempData["action"].ToString();

                return RedirectToAction(action, controller);
            } else {
                ViewData["MENSAJE"] = "Usuario/Password incorrectos";
                return View();
            }

        }

        public async Task<IActionResult> LogOut() {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Libros", "Libros");
        }

    }
}
