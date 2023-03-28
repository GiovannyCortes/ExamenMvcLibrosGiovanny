using ExamenMvcLibrosGiovanny.Data;
using ExamenMvcLibrosGiovanny.Models;
using Microsoft.EntityFrameworkCore;

namespace ExamenMvcLibrosGiovanny.Repositories {
    public class RepositoryLibros {

        private LibrosContext context;

        public RepositoryLibros(LibrosContext context) {
            this.context = context;
        }

        #region USUARIO
        public async Task<Usuario> LoginUserAsync(string email, string password) {
            return await this.context.Usuarios.FirstOrDefaultAsync(u => u.Email == email && u.Pass == password);
        }
        #endregion

        #region LIBROS
        public async Task<List<Libro>> GetAllLibrosAsync() {
            return await this.context.Libros.ToListAsync();
        }

        public async Task<List<Libro>> GetLibrosPorGeneroAsync(int idgenero) {
            return await this.context.Libros.Where(l => l.IdGenero == idgenero).ToListAsync();
        }

        public async Task<LibrosPaginados> GetLibrosPaginadosAsync(int posicion, int? idgenero) {
            List<Libro> allLibros;
            int numpaginacion = 3;
            if (idgenero != null) {
                allLibros = await this.GetLibrosPorGeneroAsync(idgenero.Value);
            } else {
                allLibros = await this.GetAllLibrosAsync();
                numpaginacion = 10;
            }

            List<Libro> librospaginados = allLibros.Skip(posicion).Take(numpaginacion).ToList();
            int numeroRegistros = allLibros.Count();

            LibrosPaginados result = new LibrosPaginados {
                Libros = librospaginados,
                NumRegistros = numeroRegistros
            };

            return result;
        }

        public async Task<Libro> FindLibroByIdAsync(int idlibro) {
            return await this.context.Libros.FirstOrDefaultAsync(l => l.IdLibro == idlibro);
        }
        #endregion

        #region GENEROS
        public async Task<List<Genero>> GetAllGenerosAsync() {
            return await this.context.Generos.ToListAsync();
        }

        public async Task<Genero> GetGeneroAsync(int idgenero) {
            return await this.context.Generos.FirstOrDefaultAsync(g => g.IdGenero == idgenero);
        }
        #endregion

        #region PEDIDOS
        public async Task ExecuteOrderAsync(int idlibro, int iduser) {
            var newid = this.context.Pedidos.Any() ? this.context.Pedidos.Max(p => p.IdPedido) + 1 : 1;
            var newidfact = this.context.Pedidos.Any() ? this.context.Pedidos.Max(p => p.IdFactura) + 1 : 1;

            DateTime currentDate = DateTime.Now;

            Pedido pedido = new Pedido() {
                IdPedido = newid,
                IdFactura = newidfact,
                Fecha = currentDate,
                IdLibro = idlibro,
                IdUsuario = iduser,
                Cantidad = 1
            };

            this.context.Pedidos.Add(pedido);
            await this.context.SaveChangesAsync();
        }
        #endregion

        #region VistaPedidos
        public async Task<List<VistaPedido>> GetVistaPedidoByIdUserAsync(int iduser) {
            return await this.context.VistaPedidos.Where(v => v.IdUsuario == iduser).ToListAsync();
        }
        #endregion

    }
}
