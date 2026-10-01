using SafeSignal.Api.DTOs.Users;

namespace SafeSignal.Api.DTOs.Auth;

/// <summary>
/// DTO de Respuesta: Token JWT y datos del usuario autenticado.
/// </summary>
public class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = "Bearer";
    public DateTime ExpiresAt { get; set; }
    public UserResponse User { get; set; } = new();
}
