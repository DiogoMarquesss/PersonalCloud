# PersonalCloud

A self-hosted personal cloud storage backend built with C# and ASP.NET Core.

Deploy your own PersonalCloud instance to register users, sign in, and upload and retrieve files. Each file is associated with its owner and stored on the host running the Files API.

> **Status:** early development. The initial APIs, authentication flow, and local file storage are in place.

## Current features

- User registration, login, logout, and current-user endpoint
- Cookie-based authentication shared by the APIs
- Authenticated file upload and download
- PostgreSQL persistence with Entity Framework Core migrations
- Local disk storage for uploaded files
- Swagger UI in the Development environment

## Tech

- C# / ASP.NET Core (.NET 10)
- PostgreSQL with EF Core
- Docker Compose for local PostgreSQL

## Getting started

Requirements: .NET 10 SDK, Docker with Docker Compose, and the `dotnet-ef` tool.

From the repository root, create your local environment file and set a secure database password:

```powershell
Copy-Item src/.env.example src/.env
```

Start PostgreSQL:

```powershell
docker compose --env-file src/.env -f src/Docker/compose.example.yaml up -d
```

The Compose file creates the users database. Create the files database too (adjust the user and database names if you changed the `.env` values):

```powershell
docker exec personalcloud-postgres psql -U personalcloud -c 'CREATE DATABASE "PersonalCloud_Files";'
```

Install `dotnet-ef` if needed, then apply both sets of migrations from the `src` directory:

```powershell
dotnet tool install --global dotnet-ef
dotnet ef database update --project PersonalCloud.Users.Data --startup-project PersonalCloud.Users.Api
dotnet ef database update --project PersonalCloud.Files.Data --startup-project PersonalCloud.Files.Api
```

Run each API in a separate terminal from `src`:

```powershell
dotnet run --project PersonalCloud.Users.Api
dotnet run --project PersonalCloud.Files.Api
```

Swagger is available in Development at:

- Users API: <https://localhost:7293/swagger> or <http://localhost:5264/swagger>
- Files API: <https://localhost:7182/swagger> or <http://localhost:5225/swagger>

## API

| Method | Endpoint | Description |
| --- | --- | --- |
| `POST` | `/api/auth/register` | Register a user. |
| `POST` | `/api/auth/login` | Sign in and receive an authentication cookie. |
| `GET` | `/api/auth/me` | Get the current authenticated user. |
| `POST` | `/api/auth/logout` | Sign out. |
| `POST` | `/api/files/upload` | Upload files as `multipart/form-data` using the `files` field. |
| `GET` | `/api/files/{id}/content` | Download a file owned by the authenticated user. |

File endpoints require the authentication cookie. Configure the storage directory with `STORAGE_BASE_PATH` and the shared ASP.NET Core Data Protection keys with `DATA_PROTECTION_PATH` in `src/.env`.

## Structure

```text
PersonalCloud/
├── src/
│   ├── PersonalCloud.Common/      # Shared code and .env loading
│   ├── PersonalCloud.Files.Api/   # File upload and download API
│   ├── PersonalCloud.Files.Data/  # File data access and migrations
│   ├── PersonalCloud.Users.Api/   # Registration and authentication API
│   ├── PersonalCloud.Users.Data/  # User data access and migrations
│   ├── Docker/
│   │   └── compose.example.yaml   # PostgreSQL development setup
│   └── .env.example               # Environment variable template
├── Local/                         # Local file storage (Git ignored)
├── Keys/                          # Data Protection keys
└── PersonalCloud.slnx
```
