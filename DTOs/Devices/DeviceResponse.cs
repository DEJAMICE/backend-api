namespace SafeSignal.Api.DTOs.Devices;

/// <summary>
/// DTO de Respuesta: Dispositivo IoT vinculado.
/// </summary>
public class DeviceResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DeviceCode { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public string MacAddress { get; set; } = string.Empty;
    public int BatteryLevel { get; set; }
    public bool IsConnected { get; set; }
    public DateTime LinkedAt { get; set; }
    public DateTime? LastSeenAt { get; set; }
}
