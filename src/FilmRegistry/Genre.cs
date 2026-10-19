namespace FilmRegistry;

public sealed class Genre
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public List<MovieEntity> Movies { get; set; } = [];
}
