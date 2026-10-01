using SafeSignal.Api.DTOs.Alerts;

namespace SafeSignal.Api.Services;

/// <summary>
/// Contrato del Servicio de Alertas SOS y Emergencias Ciudadanas.
/// </summary>
public interface IAlertService
{
    Task<AlertResponse> EmitAlertAsync(CreateAlertRequest request, string userId, string userName);
    Task<IEnumerable<AlertResponse>> GetActiveAlertsAsync();
    Task<AlertResponse?> GetAlertByIdAsync(Guid id);
    Task<IEnumerable<AlertResponse>> GetAlertHistoryAsync(string? userId = null);
    Task<AlertResponse?> ResolveAlertAsync(Guid id, ResolveAlertRequest request);
    Task<AlertResponse?> CancelAlertAsync(Guid id, CancelAlertRequest request);
}
