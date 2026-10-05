using SafeSignal.Api.Domain.Entities;

namespace SafeSignal.Api.Services.Security;

/// <summary>
/// Contrato para la emisión de tokens JWT de acceso.
/// </summary>
public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(User user);
}
