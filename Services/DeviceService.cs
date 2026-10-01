using System.Collections.Concurrent;
using SafeSignal.Api.Domain.Entities;
using SafeSignal.Api.Domain.Enums;
using SafeSignal.Api.DTOs.Devices;
using SafeSignal.Api.Services.Exceptions;

namespace SafeSignal.Api.Services;

/// <summary>
/// Implementación de la lógica de negocio de Dispositivos IoT (almacenamiento en memoria).
/// </summary>
public class DeviceService : IDeviceService
{
    private readonly ConcurrentDictionary<Guid, IoTDevice> _devices = new();

    public DeviceService()
    {
        // Dato semilla del usuario demo
        var seed = new IoTDevice
        {
            Id = Guid.Parse("cccccccc-0000-0000-0000-000000000001"),
            UserId = UserService.DemoUserId,
            Name = "Botón de pánico SafeSignal",
            DeviceCode = "SS-PB-0001",
            DeviceType = DeviceType.PanicButton,
            MacAddress = "AA:BB:CC:DD:EE:01",
            BatteryLevel = 88,
            IsConnected = true,
            LinkedAt = DateTime.UtcNow.AddDays(-6),
            LastSeenAt = DateTime.UtcNow.AddMinutes(-3)
        };
        _devices.TryAdd(seed.Id, seed);
    }

    public Task<DeviceResponse> RegisterAsync(Guid userId, RegisterDeviceRequest request)
    {
        var code = request.DeviceCode.Trim().ToUpperInvariant();
        var mac = NormalizeMac(request.MacAddress);

        if (_devices.Values.Any(d => d.DeviceCode == code))
        {
            throw new DuplicateResourceException($"El dispositivo con código '{code}' ya está vinculado.");
        }

        if (_devices.Values.Any(d => d.MacAddress == mac))
        {
            throw new DuplicateResourceException($"Ya existe un dispositivo con la MAC '{mac}'.");
        }

        var device = new IoTDevice
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = request.Name.Trim(),
            DeviceCode = code,
            DeviceType = request.DeviceType,
            MacAddress = mac,
            BatteryLevel = 100,
            IsConnected = false,
            LinkedAt = DateTime.UtcNow
        };

        _devices.TryAdd(device.Id, device);
        return Task.FromResult(MapToResponse(device));
    }

    public Task<IEnumerable<DeviceResponse>> GetAllAsync(Guid userId)
    {
        var result = _devices.Values
            .Where(d => d.UserId == userId)
            .OrderBy(d => d.LinkedAt)
            .Select(MapToResponse)
            .ToList();

        return Task.FromResult<IEnumerable<DeviceResponse>>(result);
    }

    public Task<DeviceResponse?> GetByIdAsync(Guid userId, Guid deviceId)
    {
        var device = FindOwned(userId, deviceId);
        return Task.FromResult(device == null ? null : MapToResponse(device));
    }

    public Task<DeviceResponse?> UpdateAsync(Guid userId, Guid deviceId, UpdateDeviceRequest request)
    {
        var device = FindOwned(userId, deviceId);
        if (device == null)
        {
            return Task.FromResult<DeviceResponse?>(null);
        }

        device.Name = request.Name.Trim();
        device.DeviceType = request.DeviceType;
        return Task.FromResult<DeviceResponse?>(MapToResponse(device));
    }

    public Task<DeviceResponse?> UpdateStatusAsync(Guid userId, Guid deviceId, UpdateDeviceStatusRequest request)
    {
        var device = FindOwned(userId, deviceId);
        if (device == null)
        {
            return Task.FromResult<DeviceResponse?>(null);
        }

        device.BatteryLevel = request.BatteryLevel;
        device.IsConnected = request.IsConnected;
        device.LastSeenAt = DateTime.UtcNow;
        return Task.FromResult<DeviceResponse?>(MapToResponse(device));
    }

    public Task<bool> DeleteAsync(Guid userId, Guid deviceId)
    {
        var device = FindOwned(userId, deviceId);
        return Task.FromResult(device != null && _devices.TryRemove(deviceId, out _));
    }

    public Task DeleteAllByUserAsync(Guid userId)
    {
        foreach (var id in _devices.Values.Where(d => d.UserId == userId).Select(d => d.Id).ToList())
        {
            _devices.TryRemove(id, out _);
        }

        return Task.CompletedTask;
    }

    private IoTDevice? FindOwned(Guid userId, Guid deviceId)
    {
        return _devices.TryGetValue(deviceId, out var device) && device.UserId == userId ? device : null;
    }

    private static string NormalizeMac(string mac) => mac.Trim().Replace('-', ':').ToUpperInvariant();

    private static DeviceResponse MapToResponse(IoTDevice d) => new()
    {
        Id = d.Id,
        UserId = d.UserId,
        Name = d.Name,
        DeviceCode = d.DeviceCode,
        DeviceType = d.DeviceType.ToString(),
        MacAddress = d.MacAddress,
        BatteryLevel = d.BatteryLevel,
        IsConnected = d.IsConnected,
        LinkedAt = d.LinkedAt,
        LastSeenAt = d.LastSeenAt
    };
}
