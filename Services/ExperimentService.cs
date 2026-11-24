using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using SafeSignal.Api.Domain.Entities;
using SafeSignal.Api.DTOs.Experiments;

namespace SafeSignal.Api.Services;

/// <summary>
/// Almacén en memoria de eventos de experimentación seudonimizados (Ley N.º 29733).
/// </summary>
public class ExperimentService : IExperimentService
{
    private readonly ConcurrentQueue<ExperimentEvent> _events = new();
    private const int MaxStoredEvents = 5000;

    public Task<ExperimentEventResponse> TrackEventAsync(TrackExperimentEventRequest request, string rawIdentifier)
    {
        var ev = new ExperimentEvent
        {
            Name = request.Name.Trim(),
            UserPseudoId = Pseudonymize(rawIdentifier),
            ExperimentId = string.IsNullOrWhiteSpace(request.ExperimentId) ? null : request.ExperimentId.Trim().ToUpperInvariant(),
            Variant = string.IsNullOrWhiteSpace(request.Variant) ? null : request.Variant.Trim().ToUpperInvariant(),
            Platform = request.Platform.Trim().ToLowerInvariant(),
            AppVersion = string.IsNullOrWhiteSpace(request.AppVersion) ? "1.0.0" : request.AppVersion.Trim(),
            PayloadJson = request.Params == null ? "{}" : JsonSerializer.Serialize(request.Params),
            OccurredAt = DateTime.UtcNow
        };

        _events.Enqueue(ev);
        while (_events.Count > MaxStoredEvents && _events.TryDequeue(out _)) { }

        return Task.FromResult(MapToResponse(ev));
    }

    public Task<ExperimentSummaryResponse> GetSummaryAsync(string? experimentId = null)
    {
        var snapshot = _events.ToArray();
        if (!string.IsNullOrWhiteSpace(experimentId))
        {
            var filter = experimentId.Trim().ToUpperInvariant();
            snapshot = snapshot.Where(e => string.Equals(e.ExperimentId, filter, StringComparison.OrdinalIgnoreCase)).ToArray();
        }

        var buckets = snapshot
            .GroupBy(e => new
            {
                ExperimentId = e.ExperimentId ?? "GENERAL",
                Variant = e.Variant ?? "N/A",
                e.Name
            })
            .Select(g => new ExperimentMetricBucket
            {
                ExperimentId = g.Key.ExperimentId,
                Variant = g.Key.Variant,
                EventName = g.Key.Name,
                TotalEvents = g.Count(),
                UniqueParticipants = g.Select(x => x.UserPseudoId).Distinct().Count()
            })
            .OrderBy(b => b.ExperimentId)
            .ThenBy(b => b.Variant)
            .ThenBy(b => b.EventName)
            .ToList();

        return Task.FromResult(new ExperimentSummaryResponse
        {
            TotalEvents = snapshot.Length,
            GeneratedAt = DateTime.UtcNow,
            Buckets = buckets
        });
    }

    public Task<IReadOnlyList<ExperimentEventResponse>> GetRecentEventsAsync(int limit = 50)
    {
        var bounded = Math.Clamp(limit, 1, 200);
        var result = _events.Reverse().Take(bounded).Select(MapToResponse).ToList();
        return Task.FromResult<IReadOnlyList<ExperimentEventResponse>>(result);
    }

    private static string Pseudonymize(string rawIdentifier)
    {
        var input = string.IsNullOrWhiteSpace(rawIdentifier) ? "anonymous" : rawIdentifier.Trim();
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes($"securanet-exp:{input}"));
        return Convert.ToHexString(hash)[..16].ToLowerInvariant();
    }

    private static ExperimentEventResponse MapToResponse(ExperimentEvent ev) => new()
    {
        Id = ev.Id,
        Name = ev.Name,
        UserPseudoId = ev.UserPseudoId,
        ExperimentId = ev.ExperimentId,
        Variant = ev.Variant,
        Platform = ev.Platform,
        OccurredAt = ev.OccurredAt
    };
}
