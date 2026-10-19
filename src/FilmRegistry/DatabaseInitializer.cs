using System.Data;
using Microsoft.Data.SqlClient;

namespace FilmRegistry;

public sealed class DatabaseInitializer
{
    private readonly string _connectionString;

    public DatabaseInitializer(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void EnsureCreated()
    {
        var builder = new SqlConnectionStringBuilder(_connectionString);
        if (string.IsNullOrWhiteSpace(builder.InitialCatalog))
        {
            throw new InvalidOperationException("The connection string is missing a database name.");
        }

        CreateDatabase(builder);
        CreateTables();
        Seed();
    }

    private static void CreateDatabase(SqlConnectionStringBuilder builder)
    {
        var master = new SqlConnectionStringBuilder(builder.ConnectionString)
        {
            InitialCatalog = "master"
        };

        using var connection = new SqlConnection(master.ConnectionString);
        connection.Open();
        using var command = new SqlCommand(
            """
            IF DB_ID(@name) IS NULL
            BEGIN
                DECLARE @sql nvarchar(max) = N'CREATE DATABASE ' + QUOTENAME(@name);
                EXEC(@sql);
            END
            """,
            connection);
        command.Parameters.Add(new SqlParameter("@name", SqlDbType.NVarChar, 128) { Value = builder.InitialCatalog });
        command.ExecuteNonQuery();
    }

    private void CreateTables()
    {
        Execute(
            """
            IF OBJECT_ID(N'dbo.Genre', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.Genre
                (
                    Id int IDENTITY(1, 1) NOT NULL CONSTRAINT PK_Genre PRIMARY KEY,
                    Name nvarchar(100) NOT NULL CONSTRAINT UQ_Genre_Name UNIQUE
                );
            END

            IF OBJECT_ID(N'dbo.Movie', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.Movie
                (
                    Id int IDENTITY(1, 1) NOT NULL CONSTRAINT PK_Movie PRIMARY KEY,
                    Title nvarchar(200) NOT NULL,
                    ReleaseYear int NOT NULL,
                    GenreId int NOT NULL CONSTRAINT FK_Movie_Genre FOREIGN KEY REFERENCES dbo.Genre (Id)
                );
            END
            """);
    }

    private static readonly string[] GenreNames = ["Sci-Fi", "Horror", "Action", "Comedy"];

    private static readonly (string Title, int ReleaseYear, string Genre)[] Movies =
    [
        ("Alien", 1979, "Sci-Fi"),
        ("Blade Runner", 1982, "Sci-Fi"),
        ("The Matrix", 1999, "Sci-Fi"),
        ("Children of Men", 2006, "Sci-Fi"),
        ("Ex Machina", 2014, "Sci-Fi"),
        ("Interstellar", 2014, "Sci-Fi"),
        ("Arrival", 2016, "Sci-Fi"),
        ("The Exorcist", 1973, "Horror"),
        ("Halloween", 1978, "Horror"),
        ("The Shining", 1980, "Horror"),
        ("A Nightmare on Elm Street", 1984, "Horror"),
        ("Get Out", 2017, "Horror"),
        ("Hereditary", 2018, "Horror"),
        ("Raiders of the Lost Ark", 1981, "Action"),
        ("Die Hard", 1988, "Action"),
        ("Terminator 2: Judgment Day", 1991, "Action"),
        ("The Raid", 2011, "Action"),
        ("John Wick", 2014, "Action"),
        ("Mad Max: Fury Road", 2015, "Action"),
        ("Some Like It Hot", 1959, "Comedy"),
        ("Monty Python and the Holy Grail", 1975, "Comedy"),
        ("Groundhog Day", 1993, "Comedy"),
        ("Superbad", 2007, "Comedy"),
        ("Bridesmaids", 2011, "Comedy"),
        ("The Grand Budapest Hotel", 2014, "Comedy"),
    ];

    private void Seed()
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        foreach (var genre in GenreNames)
        {
            Execute(
                connection,
                """
                IF NOT EXISTS (SELECT 1 FROM dbo.Genre WHERE Name = @name)
                    INSERT INTO dbo.Genre (Name) VALUES (@name);
                """,
                parameters => parameters.Add(new SqlParameter("@name", SqlDbType.NVarChar, 100) { Value = genre }));
        }

        foreach (var movie in Movies)
        {
            Execute(
                connection,
                """
                IF NOT EXISTS (SELECT 1 FROM dbo.Movie WHERE Title = @title)
                    INSERT INTO dbo.Movie (Title, ReleaseYear, GenreId)
                    SELECT @title, @releaseYear, Id FROM dbo.Genre WHERE Name = @genre;
                """,
                parameters =>
                {
                    parameters.Add(new SqlParameter("@title", SqlDbType.NVarChar, 200) { Value = movie.Title });
                    parameters.Add(new SqlParameter("@releaseYear", SqlDbType.Int) { Value = movie.ReleaseYear });
                    parameters.Add(new SqlParameter("@genre", SqlDbType.NVarChar, 100) { Value = movie.Genre });
                });
        }
    }

    private static void Execute(SqlConnection connection, string sql, Action<SqlParameterCollection> addParameters)
    {
        using var command = new SqlCommand(sql, connection);
        addParameters(command.Parameters);
        command.ExecuteNonQuery();
    }

    private void Execute(string sql, Action<SqlParameterCollection>? addParameters = null)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = new SqlCommand(sql, connection);
        addParameters?.Invoke(command.Parameters);
        command.ExecuteNonQuery();
    }
}
