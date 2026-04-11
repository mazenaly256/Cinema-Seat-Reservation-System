using movie_service.Models;

namespace movie_service.ResponseDTOs;

public class MovieResponseDto
{
    public Guid MovieId { get; set; }

    public string MovieName { get; set; } = null!;

    public IEnumerable<string> GenresNames { get; set; } = null!;

    public int MovieDurationMinutes { get; set; }

    public IEnumerable<ShowtimeResponseDto>? Showtimes { get; set; }

    private MovieResponseDto()
    {
        
    }

    public static MovieResponseDto? FromModel(Movie? movie)
    {
        if (movie is null)
        {
            return null;
        }

        var movieResponseDto = new MovieResponseDto
        {
            MovieId = movie.Id,
            MovieDurationMinutes = movie.DurationMinutes,
            MovieName = movie.Name,
            GenresNames = movie.Genres?.Select(mg => mg.Genre.Name),
            Showtimes = movie.Showtimes?.Select(ShowtimeResponseDto.FromModel)
        };

        return movieResponseDto;
    }
}
