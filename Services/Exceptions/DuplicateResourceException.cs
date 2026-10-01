namespace SafeSignal.Api.Services.Exceptions;

/// <summary>
/// Se lanza cuando se intenta crear un recurso que ya existe (correo o código de dispositivo duplicado).
/// Los controllers la traducen a HTTP 409 Conflict.
/// </summary>
public class DuplicateResourceException : Exception
{
    public DuplicateResourceException(string message) : base(message)
    {
    }
}
