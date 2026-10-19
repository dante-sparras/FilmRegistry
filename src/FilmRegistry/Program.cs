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

    var choice = Console.ReadLine();
    if (choice is null)
    {
        return;
    }

    try
    {
        switch (choice.Trim())
        {
            case "1":
                ShowMovies(repository.GetAll(), "Inga filmer.");
                break;
            case "2":
                SearchMovies();
                break;
            case "3":
                AddMovie();
                break;
            case "4":
                DeleteMovie();
                break;
            case "0":
                return;
            default:
                Console.WriteLine("Ogiltigt val.");
                break;
        }
    }
    catch (SqlException exception)
    {
        Console.WriteLine($"Databasfel: {exception.Message}");
    }
}

void ShowMovies(IReadOnlyList<Movie> movies, string emptyMessage)
{
    if (movies.Count == 0)
    {
        Console.WriteLine(emptyMessage);
        return;
    }

    foreach (var movie in movies)
    {
        Console.WriteLine($"{movie.Id}. {movie}");
    }
}

void SearchMovies()
{
    Console.Write("Genre: ");
    ShowMovies(repository.SearchByGenre(Console.ReadLine() ?? ""), "Inga filmer i den genren.");
}

void AddMovie()
{
    Console.Write("Titel: ");
    var title = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(title))
    {
        Console.WriteLine("Titeln kan inte vara tom.");
        return;
    }

    Console.Write("År: ");
    if (!int.TryParse(Console.ReadLine(), out var releaseYear))
    {
        Console.WriteLine("Ogiltigt år.");
        return;
    }

    var genres = repository.GetGenres();
    if (genres.Count == 0)
    {
        Console.WriteLine("Det finns inga genrer att koppla filmen till.");
        return;
    }

    Console.WriteLine("Befintliga genrer:");
    foreach (var genre in genres)
    {
        Console.WriteLine($"- {genre}");
    }

    Console.Write("Genre: ");
    var genreName = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(genreName) || !repository.Add(title, releaseYear, genreName))
    {
        Console.WriteLine("Genren finns inte. Filmen lades inte till.");
        return;
    }

    Console.WriteLine("Filmen lades till.");
}

void DeleteMovie()
{
    var movies = repository.GetAll();
    ShowMovies(movies, "Inga filmer att ta bort.");
    if (movies.Count == 0)
    {
        return;
    }

    Console.Write("Id: ");
    if (!int.TryParse(Console.ReadLine(), out var id))
    {
        Console.WriteLine("Ogiltigt id.");
        return;
    }

    Console.WriteLine(repository.Delete(id)
        ? "Filmen togs bort."
        : "Ingen film med det id:t.");
}
