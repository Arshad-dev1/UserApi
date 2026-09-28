# User Directory

A small full stack directory application. The React and TypeScript interface signs in through the ASP.NET Core API, then manages user records stored in SQLite.

## Repository structure

```text
User.API/                 ASP.NET Core API and SQLite persistence
  Controllers/             HTTP endpoints and authentication
  DB/                      Entity Framework context, entities and seeding
  Models/                  API request and response models
  Services/                Application service implementations
    Interfaces/            Service contracts consumed by controllers
  Tests/                   API MSTest project and controller tests
User.UI/                   React, TypeScript and Vite application
UserApi.slnx               .NET solution
```

## Technology

- .NET 8, ASP.NET Core Web API, Entity Framework Core and SQLite
- JWT bearer authentication with role-based authorization
- React 18, TypeScript, React Router and Vite
- MSTest and EF Core in-memory provider for API tests; Jest and Testing Library for UI tests

## Setup and run

Install the .NET 8 SDK, Node.js and npm. From the repository root, run the API:

```powershell
dotnet run --project User.API/UserApi.csproj --launch-profile https
```

Swagger is served at `https://localhost:7256/swagger`. For local development, the demo login is:

```text
Username: admin
Password: TemporaryPassword!
```

These credentials are for the Development environment only. Do not use them in a deployed environment. Set all `Authentication` values through environment variables or a secret store in production; never commit production credentials. Required values are `Issuer`, `Audience`, `Username`, `Password`, `Role`, `TokenLifetimeMinutes`, and a random `SigningKey` of at least 32 characters. For example, PowerShell uses `$env:Authentication__SigningKey = '<random secret>'` and `$env:Authentication__Issuer = '<issuer>'`.

Start the UI in another terminal:

```powershell
cd User.UI
npm install
npm run dev
```

Vite listens on `http://localhost:5173`. Configure `VITE_API_URL` if the API uses a different address. Add any non-default UI origin to the API's `Cors:AllowedOrigins` configuration.

## Authentication and authorization

`POST /api/auth/login` accepts `{ "username": "...", "password": "..." }` and returns a signed JWT with the configured lifetime. Invalid credentials receive `401 Unauthorized`. Swagger's **Authorize** button accepts the access token returned by the login endpoint and sends it as a bearer token. The UI also stores the access token in browser local storage and sends it with requests. All `/api/users` endpoints require the role configured by `Authentication:Role`; requests without a valid token receive `401`, while authenticated identities without the required role receive `403`. `UseAuthentication` runs before `UseAuthorization`.

The demo credential check is configuration based and is intended only for a local interview/demo workflow. A production system should use a managed identity provider or a persistent account store with a dedicated password hashing implementation, refresh/revocation policy, rate limiting, and secret rotation.

## API

All directory routes use JSON and are under `/api/users`.

| Method | Route | Purpose |
| --- | --- | --- |
| POST | `/api/auth/login` | Exchange configured credentials for a JWT |
| GET | `/api/users` | List records, newest first |
| GET | `/api/users/{id}` | Retrieve one record |
| POST | `/api/users` | Create a record |
| PUT | `/api/users/{id}` | Update a record |
| DELETE | `/api/users/{id}` | Delete a record |

Server-side data annotations validate requests. The API uses `UserDbContext` for persistence and projects entities into response models so database entities are not exposed directly. The SQLite database defaults to `User.API/data/app.db`; `Database:Path` can override it. The app creates the database on startup and seeds sample directory entries when empty. Disable sample seeding with `Database:Seed=false`.

## Tests and build

From the repository root:

```powershell
dotnet test User.API/Tests/UserApiTests.csproj
dotnet build UserApi.slnx
```

For the UI:

```powershell
cd User.UI
npm test
npm run build
```

## Design notes

- The API owns validation and persisted state; the UI validation is an early usability aid.
- Controllers handle HTTP concerns and call focused services through interfaces. `UserService` owns directory use cases and persistence operations; `AuthService` owns credential checks and token creation. Request and response types keep the transport contract distinct from the persistence entity.
- Entity Framework Core's `DbContext` provides the unit-of-work/repository behavior needed here. A separate generic repository layer would add indirection without a demonstrated need.
- `IUserService` is registered as scoped because it depends on EF Core's scoped `UserDbContext`. `IAuthService` is registered as singleton because its implementation is stateless and depends only on singleton configuration. JWT bearer middleware centralizes token validation, while the authorization attribute expresses the role policy at the controller boundary.
- Cancellation tokens flow from HTTP requests into asynchronous database calls.
- The API tests exercise controller behavior against isolated in-memory databases. UI tests cover validation and page behavior.
- Naming follows .NET PascalCase for types/members and camelCase for local variables, and TypeScript's established camelCase conventions.

## AI tools

GitHub Copilot was used to assist with API and UI unit tests, UI styling, and code quality improvements, including consideration of edge cases.
