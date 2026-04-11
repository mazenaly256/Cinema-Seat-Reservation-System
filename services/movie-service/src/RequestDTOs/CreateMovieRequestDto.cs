namespace movie_service.RequestDTOs;

public class CreateMovieRequestDto
{
    public string MovieName { get; set; } = null!;

    public int DurationMinutes { get; set; }

    public IEnumerable<Guid> GenresIds { get; set; } = null!;
}
