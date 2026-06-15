# Riven Backend API

ASP.NET Core 8 REST API for the **Riven Stroke System** — emergency stroke case management with hospitals, ambulances, vital signs, AI diagnostics (ECG & CT), and JWT authentication.

## Features

- **Authentication**: JWT login/register, OTP password reset (email + SMS via Twilio)
- **Case Management**: Patients, cases, handover, status workflow, analytics
- **Clinical Data**: Vital signs, symptoms, NIHSS assessments, risk factors, medications
- **AI Integration**: ECG signal/image analysis and CT stroke prediction (Hugging Face Spaces)
- **Infrastructure**: Hospitals, ambulances (GPS tracking), notifications, attachments, audit logs

## Tech Stack

| Component | Technology |
|-----------|------------|
| Framework | ASP.NET Core 8 |
| ORM | Entity Framework Core 8 |
| Database | SQL Server (local) / PostgreSQL (production) |
| Auth | JWT Bearer + BCrypt |
| Packages | Central Package Management (`Directory.Packages.props`) |

## Project Structure

```
RivenBackend/
├── Controllers/     # REST API endpoints (18 controllers)
├── Data/            # AppDbContext
├── DTOs/            # Request/response shapes
├── Extensions/      # DI & pipeline configuration
├── Middleware/      # Global exception handler
├── Migrations/      # EF Core database migrations
├── Models/          # Entity models & auth requests
├── Services/        # Email service
└── wwwroot/uploads/ # File upload storage
```

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (local dev) or PostgreSQL (production/Railway)

### Setup

```bash
# Restore dependencies
dotnet restore

# Apply database migrations
dotnet ef database update

# Run the API
dotnet run
```

Swagger UI: `http://localhost:5090/swagger`

### Default Admin Account (auto-seeded)

| Field | Value |
|-------|-------|
| Email | `admin@riven.com` |
| Password | `Admin123!` |
| Role | Admin |

### Database Connection

Edit `appsettings.Development.json`:

```json
"DefaultConnection": "Server=localhost;Database=RivenBackendDb;Trusted_Connection=True;TrustServerCertificate=True"
```

If your SQL Server instance has a different name (e.g. `DESKTOP-A747U1F`):

```json
"DefaultConnection": "Server=DESKTOP-A747U1F;Database=RivenBackendDb;Trusted_Connection=True;TrustServerCertificate=True"
```

### Configuration

Copy `.env.example` values into `appsettings.Development.json` or use User Secrets:

```bash
dotnet user-secrets set "Jwt:Key" "your-secret-key-min-32-chars"
dotnet user-secrets set "Email:Password" "your-app-password"
dotnet user-secrets set "Twilio:AccountSid" "your-sid"
```

| Variable | Description |
|----------|-------------|
| `DATABASE_URL` | PostgreSQL connection URI (Railway format) |
| `ConnectionStrings__DefaultConnection` | SQL Server connection (local) |
| `Jwt__Key` | JWT signing secret (min 32 chars) |
| `Email__Username` / `Email__Password` | SMTP credentials |
| `Twilio__AccountSid` / `Twilio__AuthToken` | SMS OTP credentials |

## API Overview

| Endpoint | Auth | Description |
|----------|------|-------------|
| `POST /api/auth/login` | Public | Login, returns JWT |
| `POST /api/auth/register` | Public | Register new user |
| `POST /api/otp/forgot-password` | Public | Send OTP via email |
| `GET /api/cases` | JWT | List all cases |
| `POST /api/ecg/analyze` | JWT | ECG signal AI analysis |
| `POST /api/ecg/predict-stroke` | JWT | CT stroke prediction |
| `POST /api/attachments/upload` | JWT | Upload case files |

Full API documentation available at `/swagger` when running.

## Roles

- **Admin** — Full access, audit logs, roles management
- **Doctor** — Clinical data, cases, AI reports
- **Paramedic** — Cases, ambulances, vital signs

## Deployment (Railway)

Set `DATABASE_URL` environment variable. Migrations run automatically in Development; for production PostgreSQL, run:

```bash
dotnet ef database update
```

## Angular Integration

### Environment (Angular `environment.ts`)

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5090',
  uploadsUrl: 'http://localhost:5090'
};
```

### HTTP Interceptor (JWT)

```typescript
intercept(req: HttpRequest<any>, next: HttpHandler) {
  const token = localStorage.getItem('token');
  if (token) {
    req = req.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
  }
  return next.handle(req);
}
```

### Key Endpoints for Angular Screens

| Screen | API |
|--------|-----|
| Login / Register | `POST /api/auth/login`, `POST /api/auth/register` |
| Dashboard | `GET /api/cases/analytics/{hospitalId}` |
| Cases List | `GET /api/cases?page=1&pageSize=20` |
| Case Detail | `GET /api/cases/{id}/detail` |
| Hospitals Map | `GET /api/hospitals/available` |
| Notifications | `GET /api/notifications/my`, `GET /api/notifications/my/unread-count` |
| File Upload | `POST /api/attachments/upload` (FormData) |
| ECG / Stroke AI | `POST /api/ecg/analyze`, `POST /api/ecg/predict-stroke` |
| Health Check | `GET /health` |

### Response Format

Auth & OTP use `{ success, message, data? }`. CRUD endpoints return DTOs directly.

Login token path: `response.data.token`

### CORS

Configured for `http://localhost:4200` in `appsettings.json` → `Cors:AllowedOrigins`.


Private — Riven Stroke System
