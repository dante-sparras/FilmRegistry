namespace FilmRegistry;

public sealed record Movie(int Id, string Title, int ReleaseYear, string Genre)
{
    public override string ToString() => $"{Title} ({ReleaseYear}) - {Genre}";
}
