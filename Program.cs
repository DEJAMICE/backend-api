using System.Reflection;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SafeSignal.Api.Services;
using SafeSignal.Api.Services.Security;

var builder = WebApplication.CreateBuilder(args);

// 1. Inyección de Controladores y formateo JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// 2. Configuración de Swagger / OpenAPI con soporte para JWT y documentación detallada
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SafeSignal API — Plataforma de Seguridad Urbana Inteligente con IA e IoT",
        Version = "v1",
        Description = "API RESTful oficial para la plataforma SafeSignal. Gestiona el ciclo de vida de emergencias SOS, telemetría de rutas asistidas, autenticación de usuarios y dispositivos IoT.\n\n" +
                      "**Organización:** DEJAMICE\n" +
                      "**Curso:** Diseño de Experimentos de Software (UPC)\n" +
                      "**Módulos Core:**\n" +
                      "* `/api/v1/alerts`: Emisión, despacho prioritario y resolución de emergencias (Persona 3 - Mathias Cárdenas).\n" +
                      "* `/api/v1/tracking`: Monitoreo en vivo de waypoints y detección de desvíos anómalos (Persona 3 - Mathias Cárdenas).\n" +
                      "* `/api/v1/auth`: Registro e inicio de sesión con token JWT (Persona 2 - Mateo Salazar).\n" +
                      "* `/api/v1/users`: Gestión del perfil del usuario autenticado (Persona 2 - Mateo Salazar).\n" +
                      "* `/api/v1/contacts`: CRUD de contactos de confianza / red de apoyo (Persona 2 - Mateo Salazar).\n" +
                      "* `/api/v1/devices`: CRUD de dispositivos IoT vinculados (Persona 2 - Mateo Salazar).\n\n" +
                      "**Cómo probar con JWT:** ejecute `POST /api/v1/auth/login` (usuario demo: `demo@safesignal.pe` / `Demo1234!`), copie el `accessToken`, pulse **Authorize** e ingrese únicamente el token.",
        Contact = new OpenApiContact
        {
            Name = "DEJAMICE — Equipo de Desarrollo SafeSignal",
            Email = "u202316353@upc.edu.pe",
            Url = new Uri("https://github.com/DEJAMICE")
        }
    });

    // Incluye los comentarios XML (///) de los controllers y DTOs en la documentación Swagger
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Definición del esquema de seguridad JWT Bearer en Swagger UI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese únicamente el token JWT obtenido en /api/v1/auth/login (Swagger agrega el prefijo 'Bearer')."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// 3. Inyección de Dependencias (Servicios de Lógica de Negocio)
builder.Services.AddSingleton<IAlertService, AlertService>();
builder.Services.AddSingleton<ITrackingService, TrackingService>();

// 3.1 Módulo base (Persona 2): seguridad, autenticación, usuarios, contactos y dispositivos
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddSingleton<IContactService, ContactService>();
builder.Services.AddSingleton<IDeviceService, DeviceService>();

// 3.2 Autenticación JWT Bearer
var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() ?? new JwtSettings();
if (string.IsNullOrWhiteSpace(jwtSettings.Key) || jwtSettings.Key.Length < 32)
{
    throw new InvalidOperationException("Jwt:Key debe estar configurada y tener al menos 32 caracteres.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ClockSkew = TimeSpan.FromMinutes(1),
            // User.Identity.Name = Id del usuario (lo usa AlertsController para registrar el dueño de la alerta)
            NameClaimType = ClaimTypes.NameIdentifier
        };
    });

builder.Services.AddAuthorization();

// 4. Configuración de CORS para permitir conexiones desde Frontend-web y Landing-Page
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSafeSignalFrontends", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",
                "http://localhost:3000",
                "http://localhost:8080",
                "https://dejamice.github.io"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });

    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// 5. Configuración del Pipeline de Middleware HTTP
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SafeSignal API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors("AllowAll");

if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Redirección amigable de raíz hacia /swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    app.Urls.Add($"http://0.0.0.0:{port}");
}

app.Run();
