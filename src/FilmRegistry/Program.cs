using FilmRegistry;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var connectionString = configuration.GetConnectionString("FilmRegistry");
if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.WriteLine("ConnectionStrings:FilmRegistry saknas i appsettings.json.");
    return;
}

var repository = new MovieRepository(connectionString);

try
{
    repository.Connect();
}
catch (SqlException exception)
{
    Console.WriteLine($"Kunde inte ansluta till databasen: {exception.Message}");
    return;
}

Console.WriteLine("Ansluten till databasen.");
new FilmMenu(repository).Run();
