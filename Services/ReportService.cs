using System.Collections.Concurrent;
using SafeSignal.Api.Domain.Entities;
using SafeSignal.Api.DTOs.Reports;

namespace SafeSignal.Api.Services;

/// <summary>
/// Implementación de la gestión de reportes comunitarios y validaciones ciudadanas.
/// </summary>
public class ReportService : IReportService
{
    private readonly ConcurrentDictionary<Guid, CommunityReport> _reports = new();
    private readonly ConcurrentDictionary<string, bool> _userVotes = new(); // key: "reportId:userId" -> true (confirm) / false (refute)

    public ReportService()
    {
        // Incidentes semilla idénticos a los mock-ups de Lima (Reportes.png)
        var seeds = new[]
        {
            new CommunityReport
            {
                Id = Guid.NewGuid(),
                UserId = UserService.DemoUserId,
                UserName = "Vecino Alerta",
                Category = "Robo",
                Address = "Av. Túpac Amaru 1450, Comas",
                Description = "Reportan arrebato de celular a peatón cerca del paradero. Precaución al caminar solo.",
                Latitude = -11.9324,
                Longitude = -77.0512,
                ConfirmedCount = 15,
                RefutedCount = 1,
                CreatedAt = DateTime.UtcNow.AddMinutes(-40)
            },
            new CommunityReport
            {
                Id = Guid.NewGuid(),
                UserId = UserService.DemoUserId,
                UserName = "Comunidad Segura",
                Category = "Zona Oscura",
                Address = "Jr. Las Gardenias 220, Los Olivos",
                Description = "Alumbrado público apagado en toda la cuadra desde hace tres días.",
                Latitude = -11.9821,
                Longitude = -77.0734,
                ConfirmedCount = 8,
                RefutedCount = 0,
                CreatedAt = DateTime.UtcNow.AddHours(-3)
            },
            new CommunityReport
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                UserName = "Ciudadano Vigilante",
                Category = "Robo",
                SecondaryCategory = "Zona Oscura",
                Address = "Av. Universitaria 3800, SMP",
                Description = "Varios vecinos reportan asaltos recurrentes en el cruce mal iluminado.",
                Latitude = -11.9950,
                Longitude = -77.0810,
                ConfirmedCount = 23,
                RefutedCount = 2,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new CommunityReport
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                UserName = "Red Vecinal Lima",
                Category = "Acoso",
                Address = "Parque Zonal Sinchi Roca, Comas",
                Description = "Persona sospechosa merodeando la zona de juegos infantiles por la tarde.",
                Latitude = -11.9210,
                Longitude = -77.0425,
                ConfirmedCount = 6,
                RefutedCount = 0,
                CreatedAt = DateTime.UtcNow.AddDays(-1).AddHours(4)
            }
        };

        foreach (var r in seeds)
        {
            _reports.TryAdd(r.Id, r);
        }
    }

    public Task<IEnumerable<ReportResponse>> GetAllAsync(Guid? currentUserId = null)
    {
        var list = _reports.Values
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => MapToResponse(r, currentUserId))
            .ToList();

        return Task.FromResult<IEnumerable<ReportResponse>>(list);
    }

    public Task<IEnumerable<ReportResponse>> GetByUserIdAsync(Guid userId)
    {
        var list = _reports.Values
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => MapToResponse(r, userId))
            .ToList();

        return Task.FromResult<IEnumerable<ReportResponse>>(list);
    }

    public Task<ReportResponse> CreateAsync(Guid userId, string userName, CreateReportRequest request)
    {
        var report = new CommunityReport
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            UserName = string.IsNullOrWhiteSpace(userName) ? "Usuario Protegido" : userName,
            Category = request.Category?.Trim() ?? "Robo",
            SecondaryCategory = request.SecondaryCategory?.Trim(),
            Address = request.Address.Trim(),
            Description = request.Description.Trim(),
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            ConfirmedCount = 1,
            RefutedCount = 0,
            CreatedAt = DateTime.UtcNow
        };

        _reports.TryAdd(report.Id, report);
        // El autor valida su propio reporte automáticamente
        _userVotes[$"{report.Id}:{userId}"] = true;

        return Task.FromResult(MapToResponse(report, userId));
    }

    public Task<ReportResponse?> ValidateAsync(Guid reportId, Guid userId, bool isConfirm)
    {
        if (!_reports.TryGetValue(reportId, out var report))
        {
            return Task.FromResult<ReportResponse?>(null);
        }

        var voteKey = $"{reportId}:{userId}";
        if (_userVotes.TryGetValue(voteKey, out var existingVote))
        {
            // Si ya votó lo mismo, quitamos el voto
            if (existingVote == isConfirm)
            {
                _userVotes.TryRemove(voteKey, out _);
                if (isConfirm && report.ConfirmedCount > 0) report.ConfirmedCount--;
                if (!isConfirm && report.RefutedCount > 0) report.RefutedCount--;
            }
            else
            {
                // Cambia el voto
                _userVotes[voteKey] = isConfirm;
                if (isConfirm)
                {
                    report.ConfirmedCount++;
                    if (report.RefutedCount > 0) report.RefutedCount--;
                }
                else
                {
                    report.RefutedCount++;
                    if (report.ConfirmedCount > 0) report.ConfirmedCount--;
                }
            }
        }
        else
        {
            // Nuevo voto
            _userVotes[voteKey] = isConfirm;
            if (isConfirm) report.ConfirmedCount++;
            else report.RefutedCount++;
        }

        return Task.FromResult<ReportResponse?>(MapToResponse(report, userId));
    }

    public Task<bool> DeleteAsync(Guid reportId, Guid userId)
    {
        if (_reports.TryGetValue(reportId, out var report) && report.UserId == userId)
        {
            return Task.FromResult(_reports.TryRemove(reportId, out _));
        }
        return Task.FromResult(false);
    }

    private ReportResponse MapToResponse(CommunityReport r, Guid? currentUserId)
    {
        bool validated = false;
        if (currentUserId.HasValue)
        {
            validated = _userVotes.TryGetValue($"{r.Id}:{currentUserId.Value}", out var vote) && vote;
        }

        return new ReportResponse
        {
            Id = r.Id,
            UserId = r.UserId,
            UserName = r.UserName,
            Category = r.Category,
            SecondaryCategory = r.SecondaryCategory,
            Address = r.Address,
            Description = r.Description,
            Latitude = r.Latitude,
            Longitude = r.Longitude,
            ConfirmedCount = r.ConfirmedCount,
            RefutedCount = r.RefutedCount,
            CreatedAt = r.CreatedAt,
            ValidatedByMe = validated
        };
    }
}
