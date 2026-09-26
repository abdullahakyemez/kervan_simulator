using System;
using System.Collections.Generic;
using UnityEngine;
using Kervan.Domain;

namespace Kervan.Data
{
    /// <summary>
    /// Unity Editörü üzerinden iki şehir arasında seyahat rotaları tanımlamamızı sağlayan ScriptableObject.
    /// </summary>
    [CreateAssetMenu(fileName = "NewMapRoute", menuName = "Kervan/Data/Map Route", order = 7)]
    public class MapRouteDataSO : ScriptableObject
    {
        [Header("Rota Kimliği")]
        [Tooltip("Benzersiz rota kimliği (örn: bursa_iznik_highway, bursa_iznik_mountain)")]
        [SerializeField] private string _routeId = string.Empty;

        [Tooltip("Oyuncuya haritada gösterilecek yol adı")]
        [SerializeField] private string _routeName = string.Empty;

        [Tooltip("Başlangıç şehri")]
        [SerializeField] private CityDataSO? _fromCity;

        [Tooltip("Hedef şehir")]
        [SerializeField] private CityDataSO? _toCity;

        [Header("Coğrafya ve Tehlike")]
        [Tooltip("Yolun geçtiği arazi türü")]
        [SerializeField] private TerrainType _terrain = TerrainType.PavedRoad;

        [Tooltip("Yol uzunluğu (km). 0 bırakılırsa şehir koordinatlarından otomatik hesaplanır.")]
        [Min(0f)]
        [SerializeField] private float _distanceKm = 0f;

        [Tooltip("Haydut ve vahşi hayvan baskını tehlike katsayısı (%0 güvenli, %100 ölümcül)")]
        [Range(0f, 1f)]
        [SerializeField] private float _dangerFactor = 0.15f;

        [Tooltip("Köprü veya derbent geçiş vergisi (Akçe)")]
        [Min(0)]
        [SerializeField] private int _tollAkce = 0;

        [Tooltip("Bu yol çift yönlü mü? (True ise hedef şehirden geri dönüş de geçerli sayılır)")]
        [SerializeField] private bool _isBidirectional = true;

        public string RouteId => _routeId;
        public string RouteName => _routeName;
        public CityDataSO? FromCity => _fromCity;
        public CityDataSO? ToCity => _toCity;
        public TerrainType Terrain => _terrain;
        public float DistanceKm => _distanceKm;
        public float DangerFactor => _dangerFactor;
        public int TollAkce => _tollAkce;
        public bool IsBidirectional => _isBidirectional;

        /// <summary>
        /// Unity verisini saf C# MapRoute nesnesine dönüştürür.
        /// </summary>
        public MapRoute? ToDomain()
        {
            if (_fromCity == null || _toCity == null) return null;

            float effectiveDistance = _distanceKm;
            if (effectiveDistance <= 0f)
            {
                effectiveDistance = TradeCalculator.CalculateDistanceKm(
                    _fromCity.MapCoordinates.x, _fromCity.MapCoordinates.y,
                    _toCity.MapCoordinates.x, _toCity.MapCoordinates.y
                );
            }

            return new MapRoute(
                routeId: _routeId,
                routeName: _routeName,
                fromCityId: _fromCity.Id,
                toCityId: _toCity.Id,
                terrain: _terrain,
                distanceKm: effectiveDistance,
                dangerFactor: _dangerFactor,
                tollAkce: _tollAkce
            );
        }
    }

    /// <summary>
    /// Oyundaki tüm harita rotalarını toplayan merkezi veri tabanı.
    /// </summary>
    [CreateAssetMenu(fileName = "MapGraphDatabase", menuName = "Kervan/Data/Map Graph Database", order = 8)]
    public class MapGraphDataSO : ScriptableObject
    {
        [SerializeField] private List<MapRouteDataSO> _routes = new List<MapRouteDataSO>();

        public IReadOnlyList<MapRouteDataSO> Routes => _routes.AsReadOnly();

        /// <summary>
        /// Tanımlı tüm rotaları saf C# MapGraph veri yapısına dönüştürür.
        /// </summary>
        public MapGraph BuildDomainGraph()
        {
            var graph = new MapGraph();

            foreach (var routeData in _routes)
            {
                if (routeData != null)
                {
                    var domainRoute = routeData.ToDomain();
                    if (domainRoute != null)
                    {
                        graph.AddRoute(domainRoute, routeData.IsBidirectional);
                    }
                }
            }

            return graph;
        }
    }
}
