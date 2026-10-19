using System.Text;
using Microsoft.Data.SqlClient;

namespace FilmRegistry;

public sealed class FilmMenu
{
    private readonly MovieRepository _repository;

    public FilmMenu(MovieRepository repository)
    {
        _repository = repository;
    }

    public void Run()
    {
        Console.OutputEncoding = Encoding.UTF8;

        while (true)
        {
            Clear();
            DrawMenu();

            var choice = Console.ReadLine();
            if (choice is null || choice.Trim() == "0")
            {
                return;
            }

            try
            {
                switch (choice.Trim())
                {
                    case "1":
                        ShowAll();
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
                    default:
                        Message("Ogiltigt val.", ConsoleColor.Red);
                        break;
                }
            }
            catch (SqlException exception)
            {
                Message($"Databasfel: {exception.Message}", ConsoleColor.Red);
            }

            Pause();
        }
    }

    private void ShowAll()
    {
        Clear();
        Heading("Alla filmer");
        ShowMovies(_repository.GetAll(), "Inga filmer.");
    }

    private void SearchMovies()
    {
        Clear();
        Heading("Sök filmer efter genre");
        Write("  Genre: ");
        ShowMovies(_repository.SearchByGenre(Console.ReadLine() ?? ""), "Inga filmer i den genren.");
    }

    private void AddMovie()
    {
        Clear();
        Heading("Lägg till film");

        Write("  Titel: ");
        var title = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(title))
        {
            Message("Titeln kan inte vara tom.", ConsoleColor.Red);
            return;
        }

        Write("  År: ");
        if (!int.TryParse(Console.ReadLine(), out var releaseYear))
        {
            Message("Ogiltigt år.", ConsoleColor.Red);
            return;
        }

        var genres = _repository.GetGenres();
        if (genres.Count == 0)
        {
            Message("Det finns inga genrer att koppla filmen till.", ConsoleColor.Red);
            return;
        }

        WriteLine();
        WriteLine("  Genrer", ConsoleColor.DarkCyan);
        foreach (var genre in genres)
        {
            WriteLine($"    {genre}");
        }

        WriteLine();
        Write("  Genre: ");
        var genreName = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(genreName) || !_repository.Add(title, releaseYear, genreName))
        {
            Message("Genren finns inte. Filmen lades inte till.", ConsoleColor.Red);
            return;
        }

        Message("Filmen lades till.", ConsoleColor.Green);
    }

    private void DeleteMovie()
    {
        Clear();
        Heading("Ta bort film");

        var movies = _repository.GetAll();
        ShowMovies(movies, "Inga filmer att ta bort.");
        if (movies.Count == 0)
        {
            return;
        }

        Write("  Id: ");
        if (!int.TryParse(Console.ReadLine(), out var id))
        {
            Message("Ogiltigt id.", ConsoleColor.Red);
            return;
        }

        var deleted = _repository.Delete(id);
        Message(
            deleted ? "Filmen togs bort." : "Ingen film med det id:t.",
            deleted ? ConsoleColor.Green : ConsoleColor.Red);
    }

    private void ShowMovies(IReadOnlyList<Movie> movies, string emptyMessage)
    {
        if (movies.Count == 0)
        {
            Message(emptyMessage, ConsoleColor.DarkYellow);
            return;
        }

        var idWidth = Math.Max(2, movies.Max(movie => movie.Id.ToString().Length));
        var titleWidth = Math.Max("Titel".Length, movies.Max(movie => movie.Title.Length));
        var genreWidth = Math.Max("Genre".Length, movies.Max(movie => movie.Genre.Length));

        WriteLine();
        WriteLine(
            $"  {"Id".PadLeft(idWidth)}  {"Titel".PadRight(titleWidth)}  År    {"Genre".PadRight(genreWidth)}",
            ConsoleColor.DarkCyan);
        WriteLine(
            $"  {new string('─', idWidth)}  {new string('─', titleWidth)}  ────  {new string('─', genreWidth)}",
            ConsoleColor.DarkGray);

        foreach (var movie in movies)
        {
            Write("  ");
            Write(movie.Id.ToString().PadLeft(idWidth), ConsoleColor.DarkYellow);
            Write($"  {movie.Title.PadRight(titleWidth)}  {movie.ReleaseYear,-4}  {movie.Genre}");
            WriteLine();
        }

        WriteLine();
        var label = movies.Count == 1 ? "1 film" : $"{movies.Count} filmer";
        WriteLine($"  {label}", ConsoleColor.DarkGray);
    }

    private void DrawMenu()
    {
        const string title = "FILMREGISTER";
        const int width = 34;
        var line = new string('─', width);

        WriteLine();
        WriteLine($"  ┌{line}┐", ConsoleColor.Cyan);
        WriteLine($"  │{title.PadLeft((width + title.Length) / 2).PadRight(width)}│", ConsoleColor.Cyan);
        WriteLine($"  └{line}┘", ConsoleColor.Cyan);
        WriteLine("  Ansluten till databasen.", ConsoleColor.DarkGray);
        WriteLine();
        WriteMenuItem("1", "Visa alla filmer");
        WriteMenuItem("2", "Sök filmer efter genre");
        WriteMenuItem("3", "Lägg till film");
        WriteMenuItem("4", "Ta bort film");
        WriteMenuItem("0", "Avsluta");
        WriteLine();
        Write("  Välj: ", ConsoleColor.Cyan);
    }

    private static void WriteMenuItem(string key, string label)
    {
        Write("   ");
        Write(key, ConsoleColor.Yellow);
        WriteLine($"   {label}");
    }

    private static void Heading(string text)
    {
        WriteLine();
        WriteLine($"  {text}", ConsoleColor.Cyan);
        WriteLine($"  {new string('─', text.Length)}", ConsoleColor.DarkCyan);
    }

    private static void Message(string text, ConsoleColor color)
    {
        WriteLine();
        WriteLine($"  {text}", color);
    }

    private static void Pause()
    {
        WriteLine();
        Write("  Tryck Enter för att fortsätta...", ConsoleColor.DarkGray);
        Console.ReadLine();
    }

    private static void Clear()
    {
        if (!Console.IsOutputRedirected)
        {
            Console.Clear();
        }
    }

    private static void Write(string text, ConsoleColor? color = null)
    {
        if (color is null || Console.IsOutputRedirected)
        {
            Console.Write(text);
            return;
        }

        var previous = Console.ForegroundColor;
        Console.ForegroundColor = color.Value;
        Console.Write(text);
        Console.ForegroundColor = previous;
    }

    private static void WriteLine(string text = "", ConsoleColor? color = null)
    {
        Write(text, color);
        Console.WriteLine();
    }
}
