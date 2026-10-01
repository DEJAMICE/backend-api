using SafeSignal.Api.Domain.Enums;

namespace SafeSignal.Api.Domain.Entities;

/// <summary>
/// Entidad del Dominio: Dispositivo IoT (botón de pánico, reloj, tag) vinculado a un usuario.
/// </summary>
public class IoTDevice
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DeviceCode { get; set; } = string.Empty;
    public DeviceType DeviceType { get; set; } = DeviceType.PanicButton;
    public string MacAddress { get; set; } = string.Empty;
    public int BatteryLevel { get; set; } = 100;
    public bool IsConnected { get; set; }
    public DateTime LinkedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastSeenAt { get; set; }
}
