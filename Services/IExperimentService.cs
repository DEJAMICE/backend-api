using SafeSignal.Api.DTOs.Experiments;

namespace SafeSignal.Api.Services;

/// <summary>
/// Servicio para registrar y consolidar eventos de experimentación sin PII.
/// </summary>
public interface IExperimentService
{
    Task<ExperimentEventResponse> TrackEventAsync(TrackExperimentEventRequest request, string rawIdentifier);
    Task<ExperimentSummaryResponse> GetSummaryAsync(string? experimentId = null);
    Task<IReadOnlyList<ExperimentEventResponse>> GetRecentEventsAsync(int limit = 50);
}
