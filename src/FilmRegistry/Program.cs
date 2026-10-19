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

while (true)
{
    Console.WriteLine();
    Console.WriteLine("--- Filmregister ---");
    Console.WriteLine("1. Visa alla filmer");
    Console.WriteLine("2. Sök filmer efter genre");
    Console.WriteLine("3. Lägg till film");
    Console.WriteLine("4. Ta bort film");
    Console.WriteLine("0. Avsluta");
    Console.Write("Välj: ");

    switch (Console.ReadLine())
    {
        case "1":
        case "2":
        case "3":
        case "4":
            Console.WriteLine("Den här funktionen är inte kopplad till databasen ännu.");
            break;
        case "0":
            return;
        default:
            Console.WriteLine("Ogiltigt val.");
            break;
    }
}
