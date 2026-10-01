using SafeSignal.Api.DTOs.Auth;

namespace SafeSignal.Api.Services;

/// <summary>
/// Contrato del Servicio de Autenticación (registro e inicio de sesión con JWT).
/// </summary>
public interface IAuthService
{
    /// <summary>Registra al usuario y devuelve su token. Lanza DuplicateResourceException si el correo existe.</summary>
    Task<AuthResponse> RegisterAsync(RegisterRequest request);

    /// <summary>Valida credenciales y devuelve el token, o null si son incorrectas.</summary>
    Task<AuthResponse?> LoginAsync(LoginRequest request);
}
