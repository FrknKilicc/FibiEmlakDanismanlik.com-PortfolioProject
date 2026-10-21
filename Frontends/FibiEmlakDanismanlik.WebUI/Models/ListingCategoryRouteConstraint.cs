using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FibiEmlakDanismanlik.WebUI.Models
{
    public class ListingCategoryRouteConstraint : IRouteConstraint
    {
        public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (values.TryGetValue(routeKey, out var value) && value != null)
            {
                var category = value.ToString();
                return string.Equals(category, "konut", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(category, "arsa", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(category, "isyeri", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(category, "ilan", StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }
    }
}
