using System.Collections.Concurrent;
using SafeSignal.Api.Domain.Entities;
using SafeSignal.Api.Domain.Enums;
using SafeSignal.Api.DTOs.Auth;
using SafeSignal.Api.DTOs.Users;
using SafeSignal.Api.Services.Exceptions;
using SafeSignal.Api.Services.Security;

namespace SafeSignal.Api.Services;

/// <summary>
/// Implementación de la lógica de negocio de Usuarios y Perfiles (almacenamiento en memoria).
/// </summary>
public class UserService : IUserService
{
    /// <summary>Id fijo del usuario de demostración (útil para pruebas en Swagger / Postman).</summary>
    public static readonly Guid DemoUserId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001");
    public const string DemoEmail = "demo@safesignal.pe";
    public const string DemoPassword = "Demo1234!";

    private readonly ConcurrentDictionary<Guid, User> _users = new();
    private readonly IPasswordHasher _hasher;

    public UserService(IPasswordHasher hasher)
    {
        _hasher = hasher;

        // Usuario semilla para demostración inmediata
        var demo = new User
        {
            Id = DemoUserId,
            FullName = "Usuario Demo SafeSignal",
            Email = DemoEmail,
            PhoneNumber = "+51999111222",
            PasswordHash = _hasher.Hash(DemoPassword),
            ProfileType = UserProfile.Student,
            SubscriptionPlan = SubscriptionPlan.Free,
            CreatedAt = DateTime.UtcNow.AddDays(-7)
        };
        _users.TryAdd(demo.Id, demo);
    }

    public Task<User> CreateAsync(RegisterRequest request)
    {
        var email = NormalizeEmail(request.Email);
        if (_users.Values.Any(u => u.Email == email))
        {
            throw new DuplicateResourceException($"El correo '{email}' ya está registrado.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName.Trim(),
            Email = email,
            PhoneNumber = request.PhoneNumber.Trim(),
            PasswordHash = _hasher.Hash(request.Password),
            ProfileType = request.ProfileType,
            SubscriptionPlan = SubscriptionPlan.Free,
            CreatedAt = DateTime.UtcNow
        };

        _users.TryAdd(user.Id, user);
        return Task.FromResult(user);
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        var normalized = NormalizeEmail(email);
        var user = _users.Values.FirstOrDefault(u => u.Email == normalized);
        return Task.FromResult(user);
    }

    public Task<UserResponse?> GetProfileAsync(Guid id)
    {
        _users.TryGetValue(id, out var user);
        return Task.FromResult(user == null ? null : MapToResponse(user));
    }

    public Task<UserResponse?> UpdateProfileAsync(Guid id, UpdateUserRequest request)
    {
        if (!_users.TryGetValue(id, out var user))
        {
            return Task.FromResult<UserResponse?>(null);
        }

        user.FullName = request.FullName.Trim();
        user.PhoneNumber = request.PhoneNumber.Trim();
        user.ProfileType = request.ProfileType;
        user.UpdatedAt = DateTime.UtcNow;

        return Task.FromResult<UserResponse?>(MapToResponse(user));
    }

    public Task<ChangePasswordResult> ChangePasswordAsync(Guid id, ChangePasswordRequest request)
    {
        if (!_users.TryGetValue(id, out var user))
        {
            return Task.FromResult(ChangePasswordResult.UserNotFound);
        }

        if (!_hasher.Verify(request.CurrentPassword, user.PasswordHash))
        {
            return Task.FromResult(ChangePasswordResult.InvalidCurrentPassword);
        }

        user.PasswordHash = _hasher.Hash(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        return Task.FromResult(ChangePasswordResult.Success);
    }

    public Task<UserResponse?> UpdateSubscriptionAsync(Guid id, UpdateSubscriptionRequest request)
    {
        if (!_users.TryGetValue(id, out var user))
        {
            return Task.FromResult<UserResponse?>(null);
        }

        user.SubscriptionPlan = request.Plan;
        user.UpdatedAt = DateTime.UtcNow;
        return Task.FromResult<UserResponse?>(MapToResponse(user));
    }

    public Task<bool> DeleteAsync(Guid id) => Task.FromResult(_users.TryRemove(id, out _));

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    public static UserResponse MapToResponse(User u) => new()
    {
        Id = u.Id,
        FullName = u.FullName,
        Email = u.Email,
        PhoneNumber = u.PhoneNumber,
        ProfileType = u.ProfileType.ToString(),
        SubscriptionPlan = u.SubscriptionPlan.ToString(),
        CreatedAt = u.CreatedAt,
        UpdatedAt = u.UpdatedAt
    };
}
