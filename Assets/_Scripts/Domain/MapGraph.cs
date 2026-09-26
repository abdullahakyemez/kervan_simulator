using System;
using System.Collections.Generic;
using System.Linq;

namespace Kervan.Domain
{
    /// <summary>
    /// 14. yüzyıl Anadolu haritasındaki şehirleri (Düğümler) ve aralarındaki yolları (Kenarlar)
    /// yöneten Graf (Graph) veri yapısı.
    /// </summary>
    public class MapGraph
    {
        // Şehir kimliği -> Dışa giden rotaların listesi (Adjacency List)
        private readonly Dictionary<string, List<MapRoute>> _routes = new Dictionary<string, List<MapRoute>>();

        /// <summary>
        /// Graf içerisine yeni bir seyahat rotası ekler.
        /// </summary>
        /// <param name="route">Eklenecek rota</param>
        /// <param name="isBidirectional">Eğer true ise, ters istikametteki rota da otomatik eklenir</param>
        public void AddRoute(MapRoute route, bool isBidirectional = true)
        {
            if (route == null) throw new ArgumentNullException(nameof(route));

            AddSingleRoute(route);

            if (isBidirectional)
            {
                var returnRoute = new MapRoute(
                    routeId: $"{route.RouteId}_return",
                    routeName: $"{route.RouteName} (Dönüş)",
                    fromCityId: route.ToCityId,
                    toCityId: route.FromCityId,
                    terrain: route.Terrain,
                    distanceKm: route.DistanceKm,
                    dangerFactor: route.DangerFactor,
                    tollAkce: route.TollAkce
                );

                AddSingleRoute(returnRoute);
            }
        }

        private void AddSingleRoute(MapRoute route)
        {
            if (!_routes.TryGetValue(route.FromCityId, out var cityRoutes))
            {
                cityRoutes = new List<MapRoute>();
                _routes[route.FromCityId] = cityRoutes;
            }

            cityRoutes.Add(route);
        }

        /// <summary>
        /// Belirli bir şehirden gidilebilecek tüm rotaları ve varış seçeneklerini listeler.
        /// </summary>
        public IReadOnlyList<MapRoute> GetAvailableRoutesFromCity(string cityId)
        {
            if (string.IsNullOrWhiteSpace(cityId) || !_routes.TryGetValue(cityId, out var routes))
            {
                return Array.Empty<MapRoute>();
            }

            return routes.AsReadOnly();
        }

        /// <summary>
        /// İki şehir arasındaki belirli bir rotayı ID'si ile bulur.
        /// </summary>
        public MapRoute? FindRoute(string fromCityId, string toCityId, string? routeId = null)
        {
            if (!_routes.TryGetValue(fromCityId, out var cityRoutes))
                return null;

            return cityRoutes.FirstOrDefault(r =>
                r.ToCityId == toCityId && (routeId == null || r.RouteId == routeId));
        }

        /// <summary>
        /// Sistemde kayıtlı tüm rotaların listesini döndürür.
        /// </summary>
        public List<MapRoute> GetAllRoutes()
        {
            return _routes.Values.SelectMany(r => r).ToList();
        }
    }
}
