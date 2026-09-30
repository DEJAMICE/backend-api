# SafeSignal — Backend RESTful API

[![Organization](https://img.shields.io/badge/Organization-DEJAMICE-blue)](https://github.com/DEJAMICE)
[![Framework](https://img.shields.io/badge/Framework-.NET%209.0%20ASP.NET%20Core-purple)](https://dotnet.microsoft.com/)
[![OpenAPI](https://img.shields.io/badge/Swagger-OpenAPI%20v1-green)](http://localhost:5000/swagger)
[![Course](https://img.shields.io/badge/UPC-Dise%C3%B1o%20de%20Experimentos%20de%20Software-red)](#)

API RESTful oficial para la plataforma de seguridad urbana inteligente **SafeSignal**, desarrollada para el curso *Diseño de Experimentos de Software* (Universidad Peruana de Ciencias Aplicadas - UPC).

---

## 🏗️ Arquitectura de la Solución

El proyecto está estructurado siguiendo principios de **Arquitectura Limpia / Por Capas**, desacoplando el dominio, la lógica de negocio y los controladores expuestos:

```text
DEJAMICE/backend-api/
├── Controllers/
│   ├── AlertsController.cs      # Endpoints /api/v1/alerts (Persona 3)
│   └── TrackingController.cs    # Endpoints /api/v1/tracking (Persona 3)
├── Domain/
│   ├── Entities/
│   │   ├── Alert.cs             # Entidad Core de Emergencia
│   │   ├── TrackingRoute.cs     # Sesión de ruta asistida
│   │   └── TrackingPoint.cs     # Waypoints geolocalizados
│   └── Enums/
│       ├── AlertStatus.cs       # Active, Resolved, Cancelled, FalseAlarm
│       ├── AlertType.cs         # PanicButton, SilentAlert, Medical, etc.
│       ├── AlertSeverity.cs     # Low, Medium, High, Critical
│       └── RouteStatus.cs       # InProgress, Completed, Interrupted
├── DTOs/
│   ├── Alerts/                  # Request/Response para alertas SOS
│   └── Tracking/                # Request/Response para telemetría
├── Services/
│   ├── IAlertService.cs         # Contrato de lógica de emergencias
│   ├── AlertService.cs          # Despacho, validación y seed data
│   ├── ITrackingService.cs      # Contrato de telemetría de rutas
│   └── TrackingService.cs       # Ingesta de puntos y cálculo de desvíos
├── Properties/
│   └── launchSettings.json      # Configurado en http://localhost:5000
└── Program.cs                   # Configuración Swagger, CORS, DI y Pipeline
```

---

## 🛡️ Endpoints Desarrollados (Persona 3 - Mathias Cárdenas)

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

## 🤝 Guía de Integración para el Equipo

* **Persona 2 (Mateo Salazar):** Puede acoplar directamente su `AuthController` (`/api/v1/auth`) y `UsersController` (`/api/v1/users`) inyectando sus servicios en `Program.cs`. La estructura ya tiene configurado el esquema de autenticación JWT Bearer en Swagger UI.
* **Personas 4 y 5 (Frontend Web):** Los endpoints coinciden 1:1 con las funciones expuestas en [`src/services/api.js`](https://github.com/DEJAMICE/Frontend-web) del repositorio `Frontend-web`.

---

## 🚀 Ejecución y Pruebas Locales

```bash
# 1. Restaurar y compilar la solución
dotnet build

# 2. Ejecutar la API en el puerto 5000
dotnet run --launch-profile http

# 3. Abrir la documentación interactiva Swagger UI en el navegador:
# 👉 http://localhost:5000/swagger
```

---

## 🌿 Metodología GitFlow

* `main`: Producción y versiones estables.
* `develop`: Integración de módulos de backend.
* `feature/Cardenas`: Desarrollo de la lógica core de Alertas SOS y Seguimiento de Rutas.