using System.Collections.Concurrent;
using SafeSignal.Api.Domain.Entities;
using SafeSignal.Api.Domain.Enums;
using SafeSignal.Api.DTOs.Tracking;

namespace SafeSignal.Api.Services;

/// <summary>
/// Implementación de la Lógica de Telemetría, Ingesta de Coordenadas y Detección de Desvíos.
/// </summary>
public class TrackingService : ITrackingService
{
    private static readonly ConcurrentDictionary<Guid, TrackingRoute> _routes = new();

    static TrackingService()
    {
        // Ruta de demostración semilla
        var seedRoute = new TrackingRoute
        {
            Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            UserId = "usr_001",
            OriginAddress = "UPC Campus San Isidro",
            DestinationAddress = "Paradero Av. Salaverry",
            Status = RouteStatus.InProgress,
            EstimatedDurationMinutes = 20,
            IsDeviationDetected = false,
            ShareWithContacts = true,
            StartedAt = DateTime.UtcNow.AddMinutes(-10),
            Waypoints = new List<TrackingPoint>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    RouteId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Latitude = -12.0890,
                    Longitude = -77.0350,
                    Speed = 4.5,
                    Heading = 45.0,
                    IsDeviation = false,
                    Timestamp = DateTime.UtcNow.AddMinutes(-8)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    RouteId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Latitude = -12.0875,
                    Longitude = -77.0335,
                    Speed = 4.8,
                    Heading = 50.0,
                    IsDeviation = false,
                    Timestamp = DateTime.UtcNow.AddMinutes(-4)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    RouteId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Latitude = -12.0864,
                    Longitude = -77.0321,
                    Speed = 5.0,
                    Heading = 55.0,
                    IsDeviation = false,
                    Timestamp = DateTime.UtcNow.AddMinutes(-1)
                }
            }
        };

        _routes.TryAdd(seedRoute.Id, seedRoute);
    }

    public Task<TrackingRouteResponse> StartTrackingAsync(StartTrackingRequest request, string userId)
    {
        var route = new TrackingRoute
        {
            Id = Guid.NewGuid(),
            UserId = string.IsNullOrWhiteSpace(userId) ? "usr_001" : userId,
            OriginAddress = request.OriginAddress,
            DestinationAddress = request.DestinationAddress,
            EstimatedDurationMinutes = request.EstimatedDurationMinutes,
            ShareWithContacts = request.ShareWithContacts,
            Status = RouteStatus.InProgress,
            StartedAt = DateTime.UtcNow
        };

        _routes.TryAdd(route.Id, route);
        return Task.FromResult(MapRouteToResponse(route));
    }

    public Task<TrackingPointResponse?> AddTrackingPointAsync(CreateTrackingPointRequest request)
    {
        if (!_routes.TryGetValue(request.RouteId, out var route))
        {
            return Task.FromResult<TrackingPointResponse?>(null);
        }

        // Lógica de detección de desvíos o anomalías de velocidad
        var isDeviation = false;
        if (route.Waypoints.Count > 0)
        {
            var lastPoint = route.Waypoints.Last();
            var distanceMeters = CalculateDistanceMeters(lastPoint.Latitude, lastPoint.Longitude, request.Latitude, request.Longitude);
            // Si el salto de posición es inusualmente grande en poco tiempo, marcar advertencia
            if (distanceMeters > 300.0)
            {
                isDeviation = true;
                route.IsDeviationDetected = true;
            }
        }

        var point = new TrackingPoint
        {
            Id = Guid.NewGuid(),
            RouteId = request.RouteId,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Speed = request.Speed,
            Heading = request.Heading,
            IsDeviation = isDeviation,
            Timestamp = DateTime.UtcNow
        };

        lock (route.Waypoints)
        {
            route.Waypoints.Add(point);
        }

        return Task.FromResult<TrackingPointResponse?>(MapPointToResponse(point));
    }

    public Task<TrackingRouteResponse?> GetLiveRouteAsync(Guid routeId)
    {
        _routes.TryGetValue(routeId, out var route);
        return Task.FromResult(route == null ? null : MapRouteToResponse(route));
    }

    public Task<TrackingRouteResponse?> StopTrackingAsync(Guid routeId)
    {
        if (!_routes.TryGetValue(routeId, out var route))
        {
            return Task.FromResult<TrackingRouteResponse?>(null);
        }

        route.Status = RouteStatus.Completed;
        route.CompletedAt = DateTime.UtcNow;

        return Task.FromResult<TrackingRouteResponse?>(MapRouteToResponse(route));
    }

    public Task<IEnumerable<TrackingRouteResponse>> GetHistoryAsync(string? userId = null)
    {
        var query = _routes.Values.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(userId))
        {
            query = query.Where(r => r.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase));
        }

        var result = query.OrderByDescending(r => r.StartedAt).Select(MapRouteToResponse);
        return Task.FromResult(result);
    }

    private static double CalculateDistanceMeters(double lat1, double lon1, double lat2, double lon2)
    {
        const double r = 6371e3; // Radio terrestre en metros
        var phi1 = lat1 * Math.PI / 180.0;
        var phi2 = lat2 * Math.PI / 180.0;
        var deltaPhi = (lat2 - lat1) * Math.PI / 180.0;
        var deltaLambda = (lon2 - lon1) * Math.PI / 180.0;

        var a = Math.Sin(deltaPhi / 2.0) * Math.Sin(deltaPhi / 2.0) +
                Math.Cos(phi1) * Math.Cos(phi2) *
                Math.Sin(deltaLambda / 2.0) * Math.Sin(deltaLambda / 2.0);

        var c = 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));
        return r * c;
    }

    private static TrackingRouteResponse MapRouteToResponse(TrackingRoute r) => new()
    {
        Id = r.Id,
        UserId = r.UserId,
        OriginAddress = r.OriginAddress,
        DestinationAddress = r.DestinationAddress,
        Status = r.Status.ToString(),
        EstimatedDurationMinutes = r.EstimatedDurationMinutes,
        IsDeviationDetected = r.IsDeviationDetected,
        ShareWithContacts = r.ShareWithContacts,
        StartedAt = r.StartedAt,
        CompletedAt = r.CompletedAt,
        Waypoints = r.Waypoints.Select(MapPointToResponse).ToList()
    };

    private static TrackingPointResponse MapPointToResponse(TrackingPoint p) => new()
    {
        Id = p.Id,
        Latitude = p.Latitude,
        Longitude = p.Longitude,
        Speed = p.Speed,
        Heading = p.Heading,
        IsDeviation = p.IsDeviation,
        Timestamp = p.Timestamp
    };
}
