namespace movie_service.Models;

public class Movie
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public int DurationMinutes { get; set; }

    public ICollection<Showtime>? Showtimes { get; set; }

    public ICollection<MovieGenre>? Genres { get; set; }
}
