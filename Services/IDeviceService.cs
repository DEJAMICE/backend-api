using SafeSignal.Api.DTOs.Devices;

namespace SafeSignal.Api.Services;

/// <summary>
/// Contrato del Servicio de Dispositivos IoT. Todas las operaciones se limitan al usuario dueño.
/// </summary>
public interface IDeviceService
{
    /// <summary>Vincula un dispositivo. Lanza DuplicateResourceException si el código o la MAC ya están registrados.</summary>
    Task<DeviceResponse> RegisterAsync(Guid userId, RegisterDeviceRequest request);
    Task<IEnumerable<DeviceResponse>> GetAllAsync(Guid userId);
    Task<DeviceResponse?> GetByIdAsync(Guid userId, Guid deviceId);
    Task<DeviceResponse?> UpdateAsync(Guid userId, Guid deviceId, UpdateDeviceRequest request);
    Task<DeviceResponse?> UpdateStatusAsync(Guid userId, Guid deviceId, UpdateDeviceStatusRequest request);
    Task<bool> DeleteAsync(Guid userId, Guid deviceId);
    Task DeleteAllByUserAsync(Guid userId);
}
