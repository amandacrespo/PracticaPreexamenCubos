using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;

namespace PracticaPreexamenCubos.Filters
{
    public class AuthorizeUsuariosAttribute: AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            string controller = context.RouteData.Values["controller"].ToString();
            string action = context.RouteData.Values["action"].ToString();
            var routeValues = context.RouteData.Values;

            // Verificar si hay algún parámetro en la URL (por ejemplo, un 'id') que debamos guardar
            var id = routeValues.ContainsKey("id") ? routeValues["id"] : null;

            // Obtener el servicio de TempData para almacenar la información de la ruta
            ITempDataProvider provider = context.HttpContext.RequestServices.GetService<ITempDataProvider>();
            var TempData = provider.LoadTempData(context.HttpContext);

            // Guardar la ruta, acción y cualquier parámetro (como el 'id') en TempData
            TempData["controller"] = controller;
            TempData["action"] = action;
            TempData["id"] = id;  // Almacenar el parámetro id si está presente

            // Guardar los cambios en TempData
            provider.SaveTempData(context.HttpContext, TempData);

            if (!user.Identity.IsAuthenticated)
            {
                context.Result = this.GetRoute("Managed", "Login");
            }
        }

        private RedirectToRouteResult GetRoute(string controller, string action)
        {
            RouteValueDictionary ruta = new RouteValueDictionary(new { controller = controller, action = action });
            RedirectToRouteResult result = new RedirectToRouteResult(ruta);
            return result;
        }
    }
}
