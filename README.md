# TaskManagementApi-JWT

Source code for **AA Programming Video #05 — JWT Authentication in ASP.NET Core Web API**.

This project demonstrates how to protect a Task Management REST API using JSON Web Tokens (JWT), with .NET 10, Entity Framework Core 10, and SQL Server.

## Features

- ASP.NET Core Web API using controllers
- Entity Framework Core with SQL Server and migrations
- `POST /api/auth/login` issues a signed JWT for valid demo credentials
- JWT validation checks issuer, audience, signing key, and token lifetime
- All `/api/tasks` endpoints require a valid Bearer token
- HTTP request examples in `TaskManagementApi.http`

## Technology stack

- .NET 10 SDK
- ASP.NET Core 10
- Entity Framework Core 10
- SQL Server
- `Microsoft.AspNetCore.Authentication.JwtBearer`
- Postman or Visual Studio Code REST Client

## Prerequisites

1. Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).
2. Install and run SQL Server.
3. Optional: install [Postman](https://www.postman.com/) or use the `.http` file in Visual Studio Code.
4. Install the EF Core CLI if it is not already installed:

```powershell
dotnet tool install --global dotnet-ef --version 10.0.13
```

If `dotnet-ef` is already installed, update it:

```powershell
dotnet tool update --global dotnet-ef --version 10.0.13
```

## 1. Configure the database

The sample connection string in `appsettings.json` is configured for local SQL Server using Windows Integrated Authentication:

```json
"DefaultConnection": "Server=.;Database=TaskManagementDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

Update `ConnectionStrings:DefaultConnection` if your SQL Server instance uses a different server name or authentication method.

Do not commit real database credentials to GitHub. For SQL Server authentication, use User Secrets or environment variables.

## 2. Initialize .NET User Secrets

Open a terminal in the directory containing `TaskManagementApi.csproj`.

Run this command **once per project**:

```powershell
dotnet user-secrets init
```

This adds a `UserSecretsId` property to the project file. If you already added `UserSecretsId` manually or initialized User Secrets before, you do not need to run this command again.

Set the demo login credentials and JWT signing key:

```powershell
dotnet user-secrets set "Jwt:Key" "REPLACE_WITH_A_RANDOM_SECRET_AT_LEAST_32_BYTES_LONG"
dotnet user-secrets set "Auth:Username" "demo"
dotnet user-secrets set "Auth:Password" "Demo123!"
```

Replace the JWT key placeholder with a strong random secret. If OpenSSL is installed, generate one with:

```powershell
openssl rand -base64 32
```

Copy the generated value into the `Jwt:Key` command. Never commit the signing key or real credentials to source control.

To confirm that the settings are stored:

```powershell
dotnet user-secrets list
```

Be careful when sharing terminal output because this command displays secret values.

The JWT issuer, audience, and token lifetime are configured in `appsettings.json`. The example access token expires after 30 minutes.

## 3. Restore packages and build

Run these commands from the directory containing `TaskManagementApi.csproj`:

```powershell
dotnet restore
dotnet build
```

Resolve any build errors before proceeding.

## 4. Create or update the database

Ensure SQL Server is running and the connection string is correct. Then run:

```powershell
dotnet ef database update
```

The project includes an initial EF Core migration.

## 5. Run the API

```powershell
dotnet run
```

The included launch profile uses:

```text
http://localhost:5180
```

In the Development environment, the OpenAPI document is available at:

```text
http://localhost:5180/openapi/v1.json
```

If the application reports that the JWT signing key is missing, make sure you ran the User Secrets commands against the correct project directory.

## 6. Test login and obtain a JWT

Send a `POST` request to:

```text
http://localhost:5180/api/auth/login
```

Set the request body to JSON:

```json
{
  "username": "demo",
  "password": "Demo123!"
}
```

A successful response contains an `accessToken`, a `tokenType` (`Bearer`), and an `expiresAtUtc` value. Copy the `accessToken` for the next request.

## 7. Call a protected endpoint

Send a `GET` request to:

```text
http://localhost:5180/api/tasks
```

Add this HTTP header, replacing the placeholder with the token from the login response:

```text
Authorization: Bearer YOUR_ACCESS_TOKEN
```

Expected behavior:

| Test | Expected result |
|---|---|
| Call `/api/tasks` without a token | `401 Unauthorized` |
| Login with incorrect credentials | `401 Unauthorized` |
| Call `/api/tasks` with a valid token | `200 OK`, assuming the database is available |
| Call an endpoint with an expired or invalid token | `401 Unauthorized` |
| Create, update, or delete a task with a valid token | Normal CRUD response |

You can also use the included `TaskManagementApi.http` file. Copy the access token from the login response into its `@token` variable before running the protected requests.

## API endpoints

| Method | Endpoint | Access |
|---|---|---|
| `POST` | `/api/auth/login` | Public; returns a JWT for valid demo credentials |
| `GET` | `/api/tasks` | Bearer token required |
| `GET` | `/api/tasks/{id}` | Bearer token required |
| `POST` | `/api/tasks` | Bearer token required |
| `PUT` | `/api/tasks/{id}` | Bearer token required |
| `DELETE` | `/api/tasks/{id}` | Bearer token required |

## Project structure

```text
TaskManagementApi-JWT/
├── Controllers/
│   ├── AuthController.cs
│   └── TasksController.cs
├── Data/
│   └── AppDbContext.cs
├── Migrations/
├── Models/
│   ├── LoginRequest.cs
│   ├── LoginResponse.cs
│   └── TaskItem.cs
├── Services/
│   ├── ITaskService.cs
│   └── TaskService.cs
├── Program.cs
├── TaskManagementApi.csproj
├── TaskManagementApi.http
├── appsettings.Development.json
└── appsettings.json
```

## Security note

This is a learning/demo implementation. `AuthController` compares one configured demo username and password. It is **not a production-ready user management system**.

For production, use a trusted identity provider with OAuth 2.0 / OpenID Connect or a proper user store with secure password hashing. Also use HTTPS, rate limiting, and managed secret storage. Never hardcode or commit production credentials or JWT signing keys.

## Related repository

Video #01 — ASP.NET Core REST API CRUD:  
https://github.com/Agunahwan/TaskManagementApi

## About AA Programming

AA Programming shares practical programming tutorials about APIs, databases, application development, and debugging.
