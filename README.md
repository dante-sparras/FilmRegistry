# Film Registry

## Requirements

- .NET 10 SDK
- SQL Server running on `localhost`

## Features

- List movies with their genre, and search by genre name.
- Add a movie to an existing genre, or delete a movie by id.
- Creates a database with 25 sample movies across 4 genres on startup when missing.

The list uses Entity Framework and LINQ. Search, add, and delete use parameterized ADO.NET.
