using Microsoft.EntityFrameworkCore;
using PracticaPreexamenCubos.Data;
using PracticaPreexamenCubos.Models;

namespace PracticaPreexamenCubos.Repositories
{
    public class RepositoryCubos
    {
        private CubosContext context;

        public RepositoryCubos(CubosContext context)
        {
            this.context = context;
        }

        #region  Autenticacion

        public async Task<Usuario> LoginUsuarioAsync(string email, string password)
        {
            return await this.context.Usuarios.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
        }

        public async Task<Usuario> FindUsuarioAsync(int idusuario)
        {
            return await this.context.Usuarios.FirstOrDefaultAsync(u => u.Id_user == idusuario);
        }

        #endregion

        #region cubos

        public async Task<List<Cubo>> GetCubosAsync()
        {
            return await this.context.Cubos.ToListAsync();
        }

        public async Task<List<Cubo>> GetCubosCarritoAsync(List<int> ids)
        {
            return await this.context.Cubos.Where(x => ids.Contains(x.Id_cubo)).ToListAsync();
        }

        public async Task FinalizarCompraAsync(int idusuario, List<int> carritoIds)
        {
            // Obtener los cubos correspondientes a los ids del carrito
            var cubos = await this.GetCubosCarritoAsync(carritoIds);

            // Contar cuántas veces aparece cada Id_cubo en el carritoIds
            var carritoContado = carritoIds.GroupBy(id => id)
                                          .ToDictionary(g => g.Key, g => g.Count()); // Esto nos da un diccionario {Id_cubo, cantidad}

            // Agrupar los cubos por Nombre y calcular el precio total
            var cubosAgrupados = cubos
                .GroupBy(c => c.Nombre)
                .Select(g => new
                {
                    Nombre = g.Key,  // El nombre del cubo
                    PrecioTotal = g.Sum(c => c.precio * carritoContado[c.Id_cubo]) // Multiplicar el precio por la cantidad
                })
                .ToList();

            foreach (var cubo in cubosAgrupados)
            {
                Compra compra = new Compra
                {
                    Id_compra = idusuario,
                    Nombre = cubo.Nombre,
                    precio = cubo.PrecioTotal,
                    fecha = DateTime.Now
                };

                this.context.Compras.Add(compra);

                await this.context.SaveChangesAsync();
            }
        }

        public async Task<List<Compra>> GetComprasUsuarioAsync(int idusuario)
        {
            return await this.context.Compras
                            .Where(c => c.Id_compra == idusuario)  // Filtramos por el Id_usuario
                            .GroupBy(c => new { c.Nombre, c.fecha })  // Agrupamos por Nombre y Fecha
                            .OrderBy(g => g.Key.fecha)  // Ordenamos por la fecha
                            .Select(g => new Compra
                            {
                                Id_compra = g.First().Id_compra,  // Tomamos el Id_compra del primer elemento del grupo
                                Nombre = g.Key.Nombre,            // El nombre del cubo (grupo)
                                fecha = g.Key.fecha,              // La fecha del grupo
                                precio = g.First().precio         // Tomamos el precio del primer elemento del grupo
                            })
                            .ToListAsync();
        }

        #endregion
    }
}
