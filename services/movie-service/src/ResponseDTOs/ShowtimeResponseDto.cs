using movie_service.Models;

namespace movie_service.ResponseDTOs;

public class ShowtimeResponseDto
{
    public Guid ShowtimeId { get; set; }

    public Guid MovieId { get; set; }

    public string MovieName { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }


    private ShowtimeResponseDto()
    {
        
    }

    public static ShowtimeResponseDto FromModel(Showtime showtime)
    {
        if (showtime is null)
        {
            return null;
        }

        var showtimeResponseDto = new ShowtimeResponseDto
        {
            ShowtimeId = showtime.Id,
            StartTime = showtime.StartTime,
            EndTime = showtime.EndTime,
            MovieId = showtime.MovieId,
            MovieName = showtime.Movie?.Name!
        };

        return showtimeResponseDto;
    }
}
