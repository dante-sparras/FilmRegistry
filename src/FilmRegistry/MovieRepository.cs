using Microsoft.Data.SqlClient;

namespace FilmRegistry;

public sealed class MovieRepository
{
    private readonly string _connectionString;

    public MovieRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Connect()
    {
        Query(
            """
            SELECT Name
            FROM dbo.Genre
            WHERE Name = @name;
            """,
            parameters => parameters.Add(new SqlParameter("@name", "Sci-Fi")),
            reader =>
            {
                while (reader.Read())
                {
                    reader.GetString(0);
                }
            });
    }

    private void Query(
        string sql,
        Action<SqlParameterCollection> addParameters,
        Action<SqlDataReader> read)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        using var command = new SqlCommand(sql, connection);
        addParameters(command.Parameters);

        using var reader = command.ExecuteReader();
        read(reader);
    }
}
