# User Directory

A small full stack user directory with a React interface and an ASP.NET Core API. The UI lets people browse, add, and edit directory entries; the API validates and stores those records in SQLite.

## What it does

- Lists users with their name, age, city, state, and pincode.
- Adds a user through a form with inline validation and clear save feedback.
- Edits a user from the list, loading the existing record before changes are submitted.
- Shows loading, empty, retry, and error states when working with the API.
- Provides a REST API for listing, reading, creating, updating, and deleting users.
- Persists records in a local SQLite database. On startup, the API creates the database if needed and seeds sample users when the database is empty. Seeding can be disabled with `Database:Seed=false`.

The API is the source of truth for data and server-side validation. The UI validates early to help people correct form input, then displays API validation errors beside the relevant fields when the server rejects a request.

## Repository layout

```text
UserApi/       ASP.NET Core API, Entity Framework Core, and SQLite
User.UI/       React, TypeScript, and Vite web application
UserApiTests/  MSTest tests for API controller behavior
```

## Requirements

- .NET 8 SDK
- Node.js and npm

## Run locally

Start the API from the repository root:

```powershell
dotnet run --project UserApi --launch-profile https
```

The HTTPS launch profile listens on `https://localhost:7256`. The API redirects to HTTPS when an HTTPS endpoint is configured. Swagger is available at `/swagger` on the API address.
`https://localhost:7256/swagger/index.html`

In a second terminal, start the UI:

```powershell
cd User.UI
npm install
npm run dev
```

Vite serves the UI at `http://localhost:5173`. Set `VITE_API_URL` in `User.UI/.env` to the API base address before starting Vite. For the HTTP launch profile above, use:

```dotenv
VITE_API_URL=http://localhost:5120
```

The UI's Vite config allows the `http://localhost:5173` origin by default. If you use a different UI origin, add it to `Cors:AllowedOrigins` in the API configuration.

To check the UI build and run its tests:

```powershell
cd User.UI
npm run build
npm test
```

To run the API tests from the repository root:

```powershell
dotnet test UserApiTests
```

## API

All routes are under `/api/users` and use JSON request and response bodies.

| Method | Route | Purpose | Success response |
| --- | --- | --- | --- |
| `GET` | `/api/users` | List users, newest first | `200 OK` |
| `GET` | `/api/users/{id}` | Get one user | `200 OK` |
| `POST` | `/api/users` | Create a user | `201 Created` with a location header |
| `PUT` | `/api/users/{id}` | Update a user | `200 OK` |
| `DELETE` | `/api/users/{id}` | Delete a user | `204 No Content` |

A user has these fields:

```json
{
  "id": 1,
  "name": "AAA AAAA",
  "age": 29,
  "city": "Melbourne",
  "state": "VIC",
  "pincode": "3000"
}
```

For create and update requests, omit `id`. The API requires a name (2–100 characters), age (1–150), city and state (2–50 characters each), and a numeric pincode (4–8 digits). Invalid requests return a validation problem response; unknown IDs return `404 Not Found`, and invalid IDs return `400 Bad Request`.

## UI commands

Run these from `User.UI`:

- `npm run dev` starts the development server.
- `npm run build` type-checks the application and creates a production bundle in `dist/`.
- `npm run preview` serves the production bundle locally.
- `npm test` runs the Jest test suite once.
- `npm run test:watch` runs Jest in watch mode.
