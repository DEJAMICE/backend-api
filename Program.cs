using Microsoft.OpenApi.Models;
using SafeSignal.Api.Services;

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
                      "* `/api/v1/auth` & `/api/v1/users`: Módulo base y autenticación JWT (Persona 2).",
        Contact = new OpenApiContact
        {
            Name = "DEJAMICE — Equipo de Desarrollo SafeSignal",
            Email = "u202316353@upc.edu.pe",
            Url = new Uri("https://github.com/DEJAMICE")
        }
    });

    // Definición del esquema de seguridad JWT Bearer en Swagger UI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT obtenido en el inicio de sesión. Ejemplo: Bearer {token}"
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Redirección amigable de raíz hacia /swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
