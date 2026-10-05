# SafeSignal — Backend RESTful API

[![Organization](https://img.shields.io/badge/Organization-DEJAMICE-blue)](https://github.com/DEJAMICE)
[![Framework](https://img.shields.io/badge/Framework-.NET%209.0%20ASP.NET%20Core-purple)](https://dotnet.microsoft.com/)
[![OpenAPI](https://img.shields.io/badge/Swagger-OpenAPI%20v1-green)](http://localhost:5000/swagger)
[![Course](https://img.shields.io/badge/UPC-Dise%C3%B1o%20de%20Experimentos%20de%20Software-red)](#)

API RESTful oficial para la plataforma de seguridad urbana inteligente **SafeSignal**, desarrollada para el curso *Diseño de Experimentos de Software* (Universidad Peruana de Ciencias Aplicadas - UPC).

---

## Arquitectura de la Solución

El proyecto está estructurado siguiendo principios de **Arquitectura Limpia / Por Capas**, desacoplando el dominio, la lógica de negocio y los controladores expuestos:

```text
DEJAMICE/backend-api/
├── Controllers/
│   ├── AuthController.cs        # Endpoints /api/v1/auth (Persona 2)
│   ├── UsersController.cs       # Endpoints /api/v1/users (Persona 2)
│   ├── ContactsController.cs    # Endpoints /api/v1/contacts (Persona 2)
│   ├── DevicesController.cs     # Endpoints /api/v1/devices (Persona 2)
│   ├── AlertsController.cs      # Endpoints /api/v1/alerts (Persona 3)
│   └── TrackingController.cs    # Endpoints /api/v1/tracking (Persona 3)
├── Domain/
│   ├── Entities/
│   │   ├── User.cs              # Usuario y perfil (Persona 2)
│   │   ├── TrustContact.cs      # Contacto de confianza (Persona 2)
│   │   ├── IoTDevice.cs         # Dispositivo IoT vinculado (Persona 2)
│   │   ├── Alert.cs             # Entidad Core de Emergencia
│   │   ├── TrackingRoute.cs     # Sesión de ruta asistida
│   │   └── TrackingPoint.cs     # Waypoints geolocalizados
│   └── Enums/
│       ├── UserProfile.cs       # Standard, Student, NightWorker
│       ├── SubscriptionPlan.cs  # Free, Premium
│       ├── AccessLevel.cs       # Primary, Secondary, EmergencyOnly
│       ├── DeviceType.cs        # PanicButton, SmartWatch, TrackerTag
│       ├── AlertStatus.cs       # Active, Resolved, Cancelled, FalseAlarm
│       ├── AlertType.cs         # PanicButton, SilentAlert, Medical, etc.
│       ├── AlertSeverity.cs     # Low, Medium, High, Critical
│       └── RouteStatus.cs       # InProgress, Completed, Interrupted
├── DTOs/
│   ├── Alerts/                  # Request/Response para alertas SOS
│   └── Tracking/                # Request/Response para telemetría
├── Services/
│   ├── Security/                # JwtSettings, JwtTokenService, PasswordHasher (PBKDF2)
│   ├── AuthService.cs / UserService.cs / ContactService.cs / DeviceService.cs  # Persona 2
│   ├── IAlertService.cs         # Contrato de lógica de emergencias
│   ├── AlertService.cs          # Despacho, validación y seed data
│   ├── ITrackingService.cs      # Contrato de telemetría de rutas
│   └── TrackingService.cs       # Ingesta de puntos y cálculo de desvíos
├── Properties/
│   └── launchSettings.json      # Configurado en http://localhost:5000
└── Program.cs                   # Configuración Swagger, CORS, DI y Pipeline
```

---

## Endpoints Desarrollados (Persona 2 - Mateo Salazar)

Los endpoints marcados con X requieren el header `Authorization: Bearer {token}`. Cada usuario solo accede a sus propios datos.

### 0. Autenticación (`/api/v1/auth`)
* `POST /api/v1/auth/register`: Registra un usuario y devuelve su token JWT.
* `POST /api/v1/auth/login`: Inicia sesión y devuelve el token JWT.

### 0.1 Usuarios y Perfiles (`/api/v1/users`) X
* `GET /api/v1/users/me`: Perfil del usuario autenticado.
* `PUT /api/v1/users/me`: Actualiza nombre, teléfono y perfil de uso (Standard / Student / NightWorker).
* `PUT /api/v1/users/me/password`: Cambia la contraseña.
* `PUT /api/v1/users/me/subscription`: Cambia el plan (Free / Premium).
* `DELETE /api/v1/users/me`: Elimina la cuenta con sus contactos y dispositivos.

### 0.2 Contactos de Confianza (`/api/v1/contacts`) X
* `POST /api/v1/contacts`: Agrega un contacto.
* `GET /api/v1/contacts?accessLevel=Primary`: Lista contactos (filtro opcional por prioridad).
* `GET /api/v1/contacts/{id}`: Detalle de un contacto.
* `PUT /api/v1/contacts/{id}`: Actualiza un contacto.
* `PATCH /api/v1/contacts/{id}/access-level`: Cambia la prioridad (Primary / Secondary / EmergencyOnly).
* `DELETE /api/v1/contacts/{id}`: Elimina un contacto.

### 0.3 Dispositivos IoT (`/api/v1/devices`) X
* `POST /api/v1/devices`: Vincula un dispositivo (código y MAC únicos).
* `GET /api/v1/devices`: Lista los dispositivos del usuario.
* `GET /api/v1/devices/{id}`: Detalle (batería, conexión, última vez visto).
* `PUT /api/v1/devices/{id}`: Edita nombre y tipo.
* `PATCH /api/v1/devices/{id}/status`: Reporta batería y conectividad.
* `DELETE /api/v1/devices/{id}`: Desvincula el dispositivo.

> **Usuario demo:** `demo@safesignal.pe` / `Demo1234!`. El almacenamiento es en memoria: los datos se reinician al detener la API.
> La clave JWT está en `appsettings.json` (sección `Jwt`). En producción debe sobrescribirse con la variable de entorno `Jwt__Key`.

## Endpoints Desarrollados (Persona 3 - Mathias Cárdenas)

### 1. Módulo de Alertas SOS (`/api/v1/alerts`)
* `POST /api/v1/alerts`: Emite una alerta de emergencia SOS geolocalizada con notificación inmediata a contactos y serenazgo.
* `GET /api/v1/alerts/active`: Lista todas las emergencias activas en tiempo real para visualización en mapa de calor.
* `GET /api/v1/alerts/{id}`: Obtiene el detalle específico de una alerta.
* `GET /api/v1/alerts/history`: Obtiene el historial de alertas emitidas (filtrable por usuario).
* `PUT /api/v1/alerts/{id}/resolve`: Da por resuelta una emergencia registrando las notas de auxilio.
* `PUT /api/v1/alerts/{id}/cancel`: Cancela una alerta activada por error o prueba.

### 2. Módulo de Monitoreo de Rutas y Telemetría (`/api/v1/tracking`)
* `POST /api/v1/tracking/start`: Inicia un recorrido seguro indicando origen, destino y contactos a notificar.
* `POST /api/v1/tracking/points`: Ingesta waypoints GPS en vivo y detecta automáticamente anomalías o desvíos (> 300m).
* `GET /api/v1/tracking/{routeId}/live`: Consulta la telemetría en tiempo real de una ruta en curso.
* `POST /api/v1/tracking/{routeId}/stop`: Finaliza la sesión de seguimiento notificando llegada segura a destino.
* `GET /api/v1/tracking/history`: Consulta el historial de trayectos del usuario.

---

## Guía de Integración para el Equipo

* **Persona 2 (Mateo Salazar):** Módulo base implementado: `/api/v1/auth`, `/api/v1/users`, `/api/v1/contacts` y `/api/v1/devices`, con autenticación JWT Bearer y documentación Swagger. Colección Postman en `docs/SafeSignal.postman_collection.json`.
* **Personas 4 y 5 (Frontend Web):** Los endpoints coinciden 1:1 con las funciones expuestas en [`src/services/api.js`](https://github.com/DEJAMICE/Frontend-web) del repositorio `Frontend-web`.

---

## Ejecución y Pruebas Locales

```bash
# 1. Restaurar y compilar la solución
dotnet build

# 2. Ejecutar la API en el puerto 5000
dotnet run --launch-profile http

# 3. Abrir la documentación interactiva Swagger UI en el navegador:
# http://localhost:5000/swagger
```

---

## Metodología GitFlow

* `main`: Producción y versiones estables.
* `develop`: Integración de módulos de backend.
* `feature/Cardenas`: Desarrollo de la lógica core de Alertas SOS y Seguimiento de Rutas.
* `feature/Salazar`: Módulo base: autenticación JWT, usuarios, contactos de confianza y dispositivos IoT.
