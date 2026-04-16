using movie_service.Models;

namespace movie_service.RequestDTOs;

public class CreateShowtimeRequestDto
{
    public Guid ShowingMovieId { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public decimal? Price { get; set; }
}
