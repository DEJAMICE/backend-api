using SafeSignal.Api.DTOs.Tracking;

namespace SafeSignal.Api.Services;

/// <summary>
/// Contrato del Servicio de Monitoreo de Rutas y Telemetría en Vivo.
/// </summary>
public interface ITrackingService
{
    Task<TrackingRouteResponse> StartTrackingAsync(StartTrackingRequest request, string userId);
    Task<TrackingPointResponse?> AddTrackingPointAsync(CreateTrackingPointRequest request);
    Task<TrackingRouteResponse?> GetLiveRouteAsync(Guid routeId);
    Task<TrackingRouteResponse?> StopTrackingAsync(Guid routeId);
    Task<IEnumerable<TrackingRouteResponse>> GetHistoryAsync(string? userId = null);
}
