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

try
{
    new DatabaseInitializer(connectionString).EnsureCreated();
}
catch (SqlException exception)
{
    Console.WriteLine($"Kunde inte förbereda databasen: {exception.Message}");
    return;
}

var repository = new MovieRepository(connectionString);
new FilmMenu(repository).Run();
