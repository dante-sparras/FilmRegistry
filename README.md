# Film Registry

## Requirements

- .NET 10 SDK
- SQL Server running on `localhost`

## Stack

| Part | Technology |
| --- | --- |
| Language | C# |
| Runtime | .NET 10 |
| Database | SQL Server |
| Data access | ADO.NET (`Microsoft.Data.SqlClient`), Entity Framework Core, and LINQ |
| Configuration | `appsettings.json` |

## Features

- List movies with their genre, and search by genre name.
- Add a movie to an existing genre, or delete a movie by id.
- Creates a database with 25 sample movies across 4 genres on startup when missing.

## Running

### Terminal

From the repository folder:

```powershell
dotnet run --project src/FilmRegistry/FilmRegistry.csproj
```

### Visual Studio

Open `FilmRegistry.slnx` and press F5.
