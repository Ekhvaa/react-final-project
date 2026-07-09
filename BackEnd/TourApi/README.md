# TourApi

ASP.NET Core Web API for a tour-booking system.

The solution is now separated into layered projects:

```text
src/TourApi.Api              ASP.NET Core host, controllers, middleware, Swagger, auth/CORS setup
src/TourApi.Core             Domain models, DTOs, services, factories, AutoMapper, repository contracts
src/TourApi.Infrastructure   EF Core DbContext, repository implementations, UnitOfWork, JWT/password infrastructure
src/TourApi.Common           Shared constants, options, exceptions
src/TourApi.Migrations       EF Core migrations and design-time factory
```

## Run locally

Start SQL Server:

```bash
docker compose up -d
```

Restore/build/run:

```bash
dotnet restore
dotnet build
dotnet run --project src/TourApi.Api
```

Open Swagger:

```text
https://localhost:<port>/swagger
```

The root `/` endpoint redirects to Swagger.

## EF Core migrations

Migrations are intentionally stored in the separate `TourApi.Migrations` project.

Add a migration:

```bash
dotnet ef migrations add MigrationName \
  --project src/TourApi.Migrations \
  --startup-project src/TourApi.Api \
  --context ApplicationDbContext
```

Apply migrations:

```bash
dotnet ef database update \
  --project src/TourApi.Migrations \
  --startup-project src/TourApi.Api \
  --context ApplicationDbContext
```

Automatic migrations are enabled for development through `src/TourApi.Api/appsettings.Development.json`:

```json
"Database": {
  "ApplyMigrationsOnStartup": true
}
```

They are disabled in the base `appsettings.json`, so production does not silently migrate unless you explicitly enable it.

## Auth roles

Current normal registration creates a tourist account by default through `/api/auth/register`.
Employee accounts use `EmployeeCreateRequest.Role`, where supported values are:

```text
TravelAgent
TourGuide
```

## Social login endpoints

The API now supports external login/register through Google and Facebook. The frontend signs in with the provider first, then sends the provider token to this API. The API verifies the provider token, creates or links a local user, then returns the same JWT response shape as `/api/auth/login`.

### Google

```http
POST /api/auth/google
```

```json
{
  "idToken": "google-id-token-from-frontend"
}
```

### Facebook

```http
POST /api/auth/facebook
```

```json
{
  "accessToken": "facebook-access-token-from-frontend"
}
```

Both endpoints return:

```json
{
  "token": "local-api-jwt",
  "expiresAtUtc": "2026-07-09T12:00:00Z",
  "userId": 1,
  "username": "user123",
  "role": "Tourist"
}
```

### Configuration

Set these in `src/TourApi.Api/appsettings.Development.json` or user secrets:

```json
{
  "Authentication": {
    "Google": {
      "ClientId": "your-google-client-id"
    },
    "Facebook": {
      "AppId": "your-facebook-app-id",
      "AppSecret": "your-facebook-app-secret"
    }
  }
}
```

External public registration creates a `Tourist` by default. Do not allow the frontend to choose `Admin`, `TravelAgent`, or `TourGuide` through social login.

### Migration

A new migration adds the `ExternalLogins` table:

```bash
dotnet ef database update --project src/TourApi.Migrations --startup-project src/TourApi.Api --context ApplicationDbContext
```


## Production backend additions

This version includes admin seeding, employee creation, refresh tokens/logout, email confirmation/password reset token storage, booking ownership checks, booking status rules, local tour/hotel image upload, and an EF migration named `AddProductionFeatures`.

See:

- `BACKEND_COMPLETION.md`
- `FRONTEND_HANDOFF.md`
- `SOCIAL_LOGIN_FRONTEND.md`


## Country flags

Country flags are no longer stored in the database as binary data and are no longer sent as Base64 in JSON.

Create a country with normal JSON:

```http
POST /api/countries
```

```json
{
  "name": "Georgia",
  "isoName": "GE"
}
```

Then upload or replace the flag as a real image file:

```http
POST /api/countries/{id}/flag
Content-Type: multipart/form-data
```

Form field:

```text
file = country flag image
```

The country response now contains `flagUrl` instead of `flagBase64`. See `COUNTRY_FLAGS.md`.

## Tests

A unit test project was added under `tests/TourApi.UnitTests`.

Run tests:

```bash
dotnet test
```

Run only unit tests:

```bash
dotnet test tests/TourApi.UnitTests/TourApi.UnitTests.csproj
```

See `TESTING.md` for covered scenarios.
