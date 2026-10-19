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
    Console.WriteLine("ConnectionStrings:FilmRegistry is missing from appsettings.json.");
    return;
}

try
{
    new DatabaseInitializer(connectionString).EnsureCreated();
}
catch (SqlException exception)
{
    Console.WriteLine($"Could not prepare the database: {exception.Message}");
    return;
}

using var context = new FilmRegistryContext(connectionString);
var repository = new MovieRepository(connectionString);
new FilmMenu(repository, context).Run();
