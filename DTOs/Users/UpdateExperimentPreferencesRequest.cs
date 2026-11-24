using System.ComponentModel.DataAnnotations;

namespace SafeSignal.Api.DTOs.Users;

/// <summary>
/// DTO para actualizar consentimiento de analítica (US26) y cuenta regresiva configurable (US17).
/// </summary>
public class UpdateExperimentPreferencesRequest
{
    public bool? AnalyticsConsentGranted { get; set; }

    [Range(5, 10, ErrorMessage = "La cuenta regresiva debe estar entre 5 y 10 segundos.")]
    public int? AutoAlertCountdownSeconds { get; set; }
}
