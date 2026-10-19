using Microsoft.EntityFrameworkCore;

namespace FilmRegistry;

public sealed class FilmRegistryContext : DbContext
{
    private readonly string _connectionString;

    public FilmRegistryContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DbSet<MovieEntity> Movies => Set<MovieEntity>();

    public DbSet<Genre> Genres => Set<Genre>();

    public IReadOnlyList<Movie> ListMovies()
    {
        return Movies
            .AsNoTracking()
            .Include(movie => movie.Genre)
            .OrderBy(movie => movie.ReleaseYear)
            .ThenBy(movie => movie.Title)
            .AsEnumerable()
            .Select(movie => new Movie(movie.Id, movie.Title, movie.ReleaseYear, movie.Genre.Name))
            .ToList();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Genre>(genre =>
        {
            genre.ToTable("Genre");
            genre.Property(item => item.Name).HasMaxLength(100).IsRequired();
            genre.HasIndex(item => item.Name).IsUnique();
        });

        modelBuilder.Entity<MovieEntity>(movie =>
        {
            movie.ToTable("Movie");
            movie.Property(item => item.Title).HasMaxLength(200).IsRequired();
            movie.HasOne(item => item.Genre)
                .WithMany(genre => genre.Movies)
                .HasForeignKey(item => item.GenreId);
        });
    }
}
