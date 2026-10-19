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
                        Message("Invalid choice.", ConsoleColor.Red);
                        break;
                }
            }
            catch (SqlException exception)
            {
                Message($"Database error: {exception.Message}", ConsoleColor.Red);
            }

            Pause();
        }
    }

    private void ShowAll()
    {
        Clear();
        Heading("All movies");
        ShowMovies(_repository.GetAll(), "No movies.");
    }

    private void SearchMovies()
    {
        Clear();
        Heading("Search movies by genre");
        Write("  Genre: ");
        ShowMovies(_repository.SearchByGenre(Console.ReadLine() ?? ""), "No movies in that genre.");
    }

    private void AddMovie()
    {
        Clear();
        Heading("Add movie");

        Write("  Title: ");
        var title = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(title))
        {
            Message("The title cannot be empty.", ConsoleColor.Red);
            return;
        }

        Write("  Year: ");
        if (!int.TryParse(Console.ReadLine(), out var releaseYear))
        {
            Message("Invalid year.", ConsoleColor.Red);
            return;
        }

        var genres = _repository.GetGenres();
        if (genres.Count == 0)
        {
            Message("There are no genres to attach the movie to.", ConsoleColor.Red);
            return;
        }

        WriteLine();
        WriteLine("  Genres", ConsoleColor.DarkCyan);
        foreach (var genre in genres)
        {
            WriteLine($"    {genre}");
        }

        WriteLine();
        Write("  Genre: ");
        var genreName = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(genreName) || !_repository.Add(title, releaseYear, genreName))
        {
            Message("That genre does not exist. The movie was not added.", ConsoleColor.Red);
            return;
        }

        Message("The movie was added.", ConsoleColor.Green);
    }

    private void DeleteMovie()
    {
        Clear();
        Heading("Delete movie");

        var movies = _repository.GetAll();
        ShowMovies(movies, "No movies to delete.");
        if (movies.Count == 0)
        {
            return;
        }

        Write("  Id: ");
        if (!int.TryParse(Console.ReadLine(), out var id))
        {
            Message("Invalid id.", ConsoleColor.Red);
            return;
        }

        var deleted = _repository.Delete(id);
        Message(
            deleted ? "The movie was deleted." : "No movie has that id.",
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
        var titleWidth = Math.Max("Title".Length, movies.Max(movie => movie.Title.Length));
        var genreWidth = Math.Max("Genre".Length, movies.Max(movie => movie.Genre.Length));

        WriteLine();
        WriteLine(
            $"  {"Id".PadLeft(idWidth)}  {"Title".PadRight(titleWidth)}  Year  {"Genre".PadRight(genreWidth)}",
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
        var label = movies.Count == 1 ? "1 movie" : $"{movies.Count} movies";
        WriteLine($"  {label}", ConsoleColor.DarkGray);
    }

    private void DrawMenu()
    {
        const string title = "FILM REGISTRY";
        const int width = 34;
        var line = new string('─', width);

        WriteLine();
        WriteLine($"  ┌{line}┐", ConsoleColor.Cyan);
        WriteLine($"  │{title.PadLeft((width + title.Length) / 2).PadRight(width)}│", ConsoleColor.Cyan);
        WriteLine($"  └{line}┘", ConsoleColor.Cyan);
        WriteLine("  Connected to the database.", ConsoleColor.DarkGray);
        WriteLine();
        WriteMenuItem("1", "Show all movies");
        WriteMenuItem("2", "Search movies by genre");
        WriteMenuItem("3", "Add movie");
        WriteMenuItem("4", "Delete movie");
        WriteMenuItem("0", "Exit");
        WriteLine();
        Write("  Choose: ", ConsoleColor.Cyan);
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
        Write("  Press Enter to continue...", ConsoleColor.DarkGray);
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
