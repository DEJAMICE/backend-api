using System.Collections.Concurrent;
using SafeSignal.Api.Domain.Entities;
using SafeSignal.Api.Domain.Enums;
using SafeSignal.Api.DTOs.Alerts;

namespace SafeSignal.Api.Services;

/// <summary>
/// Implementación de la Lógica de Negocio para el Despacho y Gestión de Alertas SOS.
/// </summary>
public class AlertService : IAlertService
{
    private static readonly ConcurrentDictionary<Guid, Alert> _alerts = new();

    static AlertService()
    {
        // Datos semilla para demostración inmediata y pruebas de Swagger / Frontend
        var seedAlert1 = new Alert
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            UserId = "usr_001",
            UserName = "Mathias Cárdenas",
            Latitude = -12.0864,
            Longitude = -77.0321,
            LocationAddress = "Av. Salaverry cdra. 24, San Isidro",
            Type = AlertType.PanicButton,
            Status = AlertStatus.Active,
            Severity = AlertSeverity.Critical,
            DeviceId = "dev_iot_01",
            DeviceName = "Pulsera SOS SafeSignal BLE",
            BatteryLevel = 88,
            ContactsNotifiedCount = 3,
            PoliceNotified = true,
            Notes = "Alerta activada desde pulsera IoT en trayecto universitario",
            CreatedAt = DateTime.UtcNow.AddMinutes(-4)
        };

        var seedAlert2 = new Alert
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            UserId = "usr_002",
            UserName = "Valeria Mendoza",
            Latitude = -12.1215,
            Longitude = -77.0298,
            LocationAddress = "Calle Las Begonias, San Isidro",
            Type = AlertType.SilentAlert,
            Status = AlertStatus.Active,
            Severity = AlertSeverity.High,
            DeviceId = "dev_iot_02",
            DeviceName = "Llavero Físico SOS",
            BatteryLevel = 94,
            ContactsNotifiedCount = 2,
            PoliceNotified = true,
            Notes = "Alerta silenciosa discreta",
            CreatedAt = DateTime.UtcNow.AddMinutes(-12)
        };

        var seedAlert3 = new Alert
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            UserId = "usr_001",
            UserName = "Mathias Cárdenas",
            Latitude = -12.0912,
            Longitude = -77.0425,
            LocationAddress = "Av. Brasil con Av. Ejército, Magdalena",
            Type = AlertType.PanicButton,
            Status = AlertStatus.Resolved,
            Severity = AlertSeverity.Medium,
            DeviceId = "dev_iot_01",
            DeviceName = "Pulsera SOS SafeSignal BLE",
            BatteryLevel = 95,
            ContactsNotifiedCount = 3,
            PoliceNotified = true,
            ResolutionNotes = "Asistencia de Serenazgo completada. Usuario fuera de peligro.",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            ResolvedAt = DateTime.UtcNow.AddDays(-1).AddMinutes(25)
        };

        _alerts.TryAdd(seedAlert1.Id, seedAlert1);
        _alerts.TryAdd(seedAlert2.Id, seedAlert2);
        _alerts.TryAdd(seedAlert3.Id, seedAlert3);
    }

    public Task<AlertResponse> EmitAlertAsync(CreateAlertRequest request, string userId, string userName)
    {
        var alert = new Alert
        {
            Id = Guid.NewGuid(),
            UserId = string.IsNullOrWhiteSpace(userId) ? "usr_001" : userId,
            UserName = string.IsNullOrWhiteSpace(userName) ? "Mathias Cárdenas" : userName,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            LocationAddress = string.IsNullOrWhiteSpace(request.LocationAddress)
                ? $"Coordenadas ({request.Latitude:F4}, {request.Longitude:F4}) - Lima"
                : request.LocationAddress,
            Type = request.Type,
            Status = AlertStatus.Active,
            Severity = request.Severity,
            DeviceId = request.DeviceId,
            DeviceName = request.DeviceName ?? "Dispositivo SafeSignal",
            BatteryLevel = request.BatteryLevel,
            ContactsNotifiedCount = 3, // Simula despacho inmediato a la red de confianza
            PoliceNotified = true,     // Simula alerta automática a la central integrada
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        _alerts.TryAdd(alert.Id, alert);
        return Task.FromResult(MapToResponse(alert));
    }

    public Task<IEnumerable<AlertResponse>> GetActiveAlertsAsync()
    {
        var activeAlerts = _alerts.Values
            .Where(a => a.Status == AlertStatus.Active)
            .OrderByDescending(a => a.CreatedAt)
            .Select(MapToResponse);

        return Task.FromResult(activeAlerts);
    }

    public Task<AlertResponse?> GetAlertByIdAsync(Guid id)
    {
        _alerts.TryGetValue(id, out var alert);
        return Task.FromResult(alert == null ? null : MapToResponse(alert));
    }

    public Task<IEnumerable<AlertResponse>> GetAlertHistoryAsync(string? userId = null)
    {
        var query = _alerts.Values.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(userId))
        {
            query = query.Where(a => a.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase));
        }

        var history = query.OrderByDescending(a => a.CreatedAt).Select(MapToResponse);
        return Task.FromResult(history);
    }

    public Task<AlertResponse?> ResolveAlertAsync(Guid id, ResolveAlertRequest request)
    {
        if (!_alerts.TryGetValue(id, out var alert))
        {
            return Task.FromResult<AlertResponse?>(null);
        }

        alert.Status = AlertStatus.Resolved;
        alert.ResolvedAt = DateTime.UtcNow;
        alert.ResolutionNotes = request.ResolutionNotes;

        return Task.FromResult<AlertResponse?>(MapToResponse(alert));
    }

    public Task<AlertResponse?> CancelAlertAsync(Guid id, CancelAlertRequest request)
    {
        if (!_alerts.TryGetValue(id, out var alert))
        {
            return Task.FromResult<AlertResponse?>(null);
        }

        alert.Status = AlertStatus.Cancelled;
        alert.CancelledAt = DateTime.UtcNow;
        alert.CancellationReason = request.Reason;

        return Task.FromResult<AlertResponse?>(MapToResponse(alert));
    }

    private static AlertResponse MapToResponse(Alert a) => new()
    {
        Id = a.Id,
        UserId = a.UserId,
        UserName = a.UserName,
        Latitude = a.Latitude,
        Longitude = a.Longitude,
        LocationAddress = a.LocationAddress,
        Type = a.Type.ToString(),
        Status = a.Status.ToString(),
        Severity = a.Severity.ToString(),
        DeviceId = a.DeviceId,
        DeviceName = a.DeviceName,
        BatteryLevel = a.BatteryLevel,
        ContactsNotifiedCount = a.ContactsNotifiedCount,
        PoliceNotified = a.PoliceNotified,
        Notes = a.Notes,
        ResolutionNotes = a.ResolutionNotes,
        CancellationReason = a.CancellationReason,
        CreatedAt = a.CreatedAt,
        ResolvedAt = a.ResolvedAt,
        CancelledAt = a.CancelledAt
    };
}
