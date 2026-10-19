using System.Data;
using Microsoft.Data.SqlClient;

namespace FilmRegistry;

public sealed class MovieRepository
{
    private const string MovieSelect = """
        SELECT m.Id, m.Title, m.ReleaseYear, g.Name
        FROM dbo.Movie AS m
        INNER JOIN dbo.Genre AS g ON g.Id = m.GenreId
        """;

    private readonly string _connectionString;

    public MovieRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IReadOnlyList<Movie> GetAll()
    {
        return ReadMovies($"{MovieSelect} ORDER BY m.ReleaseYear, m.Title;");
    }

    public IReadOnlyList<string> GetGenres()
    {
        var genres = new List<string>();
        Execute(
            "SELECT Name FROM dbo.Genre ORDER BY Name;",
            _ => { },
            command =>
            {
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    genres.Add(reader.GetString(0));
                }
            });

        return genres;
    }

    public IReadOnlyList<Movie> SearchByGenre(string genre)
    {
        if (string.IsNullOrWhiteSpace(genre))
        {
            return [];
        }

        return ReadMovies(
            $"{MovieSelect} WHERE g.Name = @genre ORDER BY m.ReleaseYear, m.Title;",
            parameters => parameters.Add(GenreParameter(genre)));
    }

    public bool Add(string title, int releaseYear, string genre)
    {
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(genre))
        {
            return false;
        }

        var inserted = 0;
        Execute(
            """
            INSERT INTO dbo.Movie (Title, ReleaseYear, GenreId)
            SELECT @title, @releaseYear, Id
            FROM dbo.Genre
            WHERE Name = @genre;
            """,
            parameters =>
            {
                parameters.Add(new SqlParameter("@title", SqlDbType.NVarChar, 200) { Value = title.Trim() });
                parameters.Add(new SqlParameter("@releaseYear", SqlDbType.Int) { Value = releaseYear });
                parameters.Add(GenreParameter(genre));
            },
            command => inserted = command.ExecuteNonQuery());

        return inserted == 1;
    }

    public bool Delete(int id)
    {
        var deleted = 0;
        Execute(
            "DELETE FROM dbo.Movie WHERE Id = @id;",
            parameters => parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id }),
            command => deleted = command.ExecuteNonQuery());

        return deleted == 1;
    }

    private List<Movie> ReadMovies(string sql, Action<SqlParameterCollection>? addParameters = null)
    {
        var movies = new List<Movie>();
        Execute(
            sql,
            addParameters ?? (_ => { }),
            command =>
            {
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    movies.Add(new Movie(
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.GetInt32(2),
                        reader.GetString(3)));
                }
            });

        return movies;
    }

    private void Execute(
        string sql,
        Action<SqlParameterCollection> addParameters,
        Action<SqlCommand> useCommand)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        using var command = new SqlCommand(sql, connection);
        addParameters(command.Parameters);
        useCommand(command);
    }

    private static SqlParameter GenreParameter(string genre)
    {
        return new SqlParameter("@genre", SqlDbType.NVarChar, 100) { Value = genre.Trim() };
    }
}
