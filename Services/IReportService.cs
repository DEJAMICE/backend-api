using SafeSignal.Api.DTOs.Reports;

namespace SafeSignal.Api.Services;

public interface IReportService
{
    Task<IEnumerable<ReportResponse>> GetAllAsync(Guid? currentUserId = null);
    Task<IEnumerable<ReportResponse>> GetByUserIdAsync(Guid userId);
    Task<ReportResponse> CreateAsync(Guid userId, string userName, CreateReportRequest request);
    Task<ReportResponse?> ValidateAsync(Guid reportId, Guid userId, bool isConfirm);
    Task<bool> DeleteAsync(Guid reportId, Guid userId);
}
