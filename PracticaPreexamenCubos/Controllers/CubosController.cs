using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using PracticaPreexamenCubos.Extensions;
using PracticaPreexamenCubos.Filters;
using PracticaPreexamenCubos.Models;
using PracticaPreexamenCubos.Repositories;

namespace PracticaPreexamenCubos.Controllers
{
    public class CubosController : Controller
    {
        private RepositoryCubos repo;

        public CubosController(RepositoryCubos repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            List<Cubo> cubos = await this.repo.GetCubosAsync();
            return View(cubos);
        }


        [AuthorizeUsuarios]
        public async Task<IActionResult> Carrito()
        {
            var carritoIds = HttpContext.Session.GetObject<List<int>>("CarritoIds");
            if (carritoIds == null)
            {
                carritoIds = new List<int>();
            }

            List<Cubo> cubos = await this.repo.GetCubosCarritoAsync(carritoIds);

            return View(cubos);
        }

        [AuthorizeUsuarios]
        public async Task<IActionResult> AddCarrito(int id, string? view)
        {
            var carritoIds = HttpContext.Session.GetObject<List<int>>("CarritoIds");
            if (carritoIds == null)
            {
                carritoIds = new List<int>();
            }

            carritoIds.Add(id);
            HttpContext.Session.SetObject("CarritoIds", carritoIds);

            if (view != null && view != "")
            {
                return RedirectToAction("Carrito");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [AuthorizeUsuarios]
        public async Task<IActionResult> RemoveCarrito(int id, string? view)
        {
            var carritoIds = HttpContext.Session.GetObject<List<int>>("CarritoIds");
            if (carritoIds == null)
            {
                carritoIds = new List<int>();
            }

            carritoIds.RemoveAt(carritoIds.IndexOf(id));
            HttpContext.Session.SetObject("CarritoIds", carritoIds);

            return RedirectToAction("Carrito");
        }

        [AuthorizeUsuarios]
        public async Task<IActionResult> DeleteCarrito(int id, string? view)
        {
            var carritoIds = HttpContext.Session.GetObject<List<int>>("CarritoIds");
            if (carritoIds == null)
            {
                carritoIds = new List<int>();
            }

            carritoIds.RemoveAll(x => x == id);
            HttpContext.Session.SetObject("CarritoIds", carritoIds);

            return RedirectToAction("Carrito");
        }

        [AuthorizeUsuarios]
        public async Task<IActionResult> FinalizarCompra()
        {
            var carritoIds = HttpContext.Session.GetObject<List<int>>("CarritoIds");
            int idusuario = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (carritoIds != null)
            {
                await this.repo.FinalizarCompraAsync(idusuario, carritoIds);
                HttpContext.Session.Remove("CarritoIds");
                return RedirectToAction("HistorialCompra");
            }
            else
            {
                return RedirectToAction("Carrito");
            }
        }

        [AuthorizeUsuarios]
        public async Task<IActionResult> HistorialCompra()
        {
            int idusuario = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            List<Compra> compras = await this.repo.GetComprasUsuarioAsync(idusuario);
            return View(compras);
        }
    }
}
