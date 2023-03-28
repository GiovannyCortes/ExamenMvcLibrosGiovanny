using ExamenMvcLibrosGiovanny.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ExamenMvcLibrosGiovanny.Controllers {
    public class UserController : Controller {

        [AuthorizeUsers]
        public IActionResult PerfilUsuario() {
            return View();
        }

    }
}
