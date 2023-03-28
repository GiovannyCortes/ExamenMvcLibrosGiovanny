using ExamenMvcLibrosGiovanny.Extensions;
using ExamenMvcLibrosGiovanny.Filters;
using ExamenMvcLibrosGiovanny.Models;
using ExamenMvcLibrosGiovanny.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ExamenMvcLibrosGiovanny.Controllers {
    public class LibrosController : Controller {

        public RepositoryLibros repo; 

        public LibrosController(RepositoryLibros repo) {
            this.repo = repo;
        }

        public async Task<IActionResult> Libros(int? posicion) {
            posicion = posicion ?? 0;
            LibrosPaginados librosPaginados = await this.repo.GetLibrosPaginadosAsync(posicion.Value, null);

            List<Libro> libros = librosPaginados.Libros;
            int numeroRegistros = librosPaginados.NumRegistros;
            ViewData["REGISTROS"] = numeroRegistros;
            return View(libros);
        }

        public async Task<IActionResult> LibrosPorGenero(int? posicion, int idgenero) {
            posicion = posicion ?? 0;
            LibrosPaginados librosPaginados = await this.repo.GetLibrosPaginadosAsync(posicion.Value, idgenero);

            List<Libro> libros = librosPaginados.Libros;

            Genero genero = await this.repo.GetGeneroAsync(idgenero);
            ViewData["GENERO"] = genero;
            
            int numeroRegistros = librosPaginados.NumRegistros;
            ViewData["REGISTROS"] = numeroRegistros;

            return View(libros);
        }

        public async Task<IActionResult> DetallesLibro(int idlibro) {
            Libro libro = await this.repo.FindLibroByIdAsync(idlibro);
            return View(libro);
        }

        public IActionResult AddItemSession(int idlibro) {
            List<int> items = HttpContext.Session.GetObject<List<int>>("ITEMS");
            if (items == null) { // No hay lista de carritos aún, la creamos
                List<int> new_list_items = new List<int> { idlibro };
                HttpContext.Session.SetObject("ITEMS", new_list_items);
            } else { // Si que hay lista, la modificamos
                if(!items.Contains(idlibro)) {
                    items.Add(idlibro);
                    HttpContext.Session.SetObject("ITEMS", items);
                }
            }
            return RedirectToAction("Libros");
        }

        public IActionResult DeleteItemSession(int idlibro) {
            List<int> items = HttpContext.Session.GetObject<List<int>>("ITEMS");
            if (items != null) { // Si que hay lista, la modificamos
                items.Remove(idlibro);
                HttpContext.Session.SetObject("ITEMS", items);
            }
            return RedirectToAction("Libros");
        }

        public async Task<IActionResult> GetItemsSession() {
            List<int> items = HttpContext.Session.GetObject<List<int>>("ITEMS");
            List<Libro> libros = new List<Libro>();
            if (items != null) {
                foreach (int idlibro in items) {
                    Libro libro = await this.repo.FindLibroByIdAsync(idlibro);
                    libros.Add(libro);
                }
            }
            return View(libros);
        }

        [AuthorizeUsers]
        public async Task<IActionResult> ExecuteOrder() {
            int iduser = int.Parse(HttpContext.User.FindFirst("ID").Value);
            List<int> items = HttpContext.Session.GetObject<List<int>>("ITEMS");
            if (items != null) {
                foreach (int idlibro in items) {
                    await this.repo.ExecuteOrderAsync(idlibro, iduser);
                }
                HttpContext.Session.Remove("ITEMS");
            }
            List<VistaPedido> pedidos = await this.repo.GetVistaPedidoByIdUserAsync(iduser);
            return View(pedidos);
        }

    }
}
