namespace movie_service.RequestDTOs;

public class UpdateMovieRequestDto
{
    public string MovieName { get; set; } = null!;

    public int DurationMinutes { get; set; }

    public ICollection<Guid> GenresIds { get; set; } = null!;
}
