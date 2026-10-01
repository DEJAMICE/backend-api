namespace SafeSignal.Api.Services.Security;

/// <summary>
/// Contrato para el hash y verificación de contraseñas.
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string storedHash);
}
