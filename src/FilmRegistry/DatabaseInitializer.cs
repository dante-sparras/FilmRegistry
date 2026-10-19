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
            throw new InvalidOperationException("Anslutningssträngen saknar ett databasnamn.");
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

    private void Seed()
    {
        Execute(
            """
            IF NOT EXISTS (SELECT 1 FROM dbo.Genre)
            BEGIN
                INSERT INTO dbo.Genre (Name)
                VALUES (@sciFi), (@horror), (@action);
            END

            IF NOT EXISTS (SELECT 1 FROM dbo.Movie)
            BEGIN
                INSERT INTO dbo.Movie (Title, ReleaseYear, GenreId)
                SELECT @alienTitle, @alienYear, Id FROM dbo.Genre WHERE Name = @sciFi;

                INSERT INTO dbo.Movie (Title, ReleaseYear, GenreId)
                SELECT @shiningTitle, @shiningYear, Id FROM dbo.Genre WHERE Name = @horror;

                INSERT INTO dbo.Movie (Title, ReleaseYear, GenreId)
                SELECT @dieHardTitle, @dieHardYear, Id FROM dbo.Genre WHERE Name = @action;
            END
            """,
            parameters =>
            {
                parameters.Add(new SqlParameter("@sciFi", SqlDbType.NVarChar, 100) { Value = "Sci-Fi" });
                parameters.Add(new SqlParameter("@horror", SqlDbType.NVarChar, 100) { Value = "Horror" });
                parameters.Add(new SqlParameter("@action", SqlDbType.NVarChar, 100) { Value = "Action" });
                parameters.Add(new SqlParameter("@alienTitle", SqlDbType.NVarChar, 200) { Value = "Alien" });
                parameters.Add(new SqlParameter("@alienYear", SqlDbType.Int) { Value = 1979 });
                parameters.Add(new SqlParameter("@shiningTitle", SqlDbType.NVarChar, 200) { Value = "The Shining" });
                parameters.Add(new SqlParameter("@shiningYear", SqlDbType.Int) { Value = 1980 });
                parameters.Add(new SqlParameter("@dieHardTitle", SqlDbType.NVarChar, 200) { Value = "Die Hard" });
                parameters.Add(new SqlParameter("@dieHardYear", SqlDbType.Int) { Value = 1988 });
            });
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
