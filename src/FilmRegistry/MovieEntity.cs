namespace FilmRegistry;

public sealed class MovieEntity
{
    public int Id { get; set; }

    public string Title { get; set; } = "";

    public int ReleaseYear { get; set; }

    public int GenreId { get; set; }

    public Genre Genre { get; set; } = null!;
}
