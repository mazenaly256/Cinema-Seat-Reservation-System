using System.ComponentModel.DataAnnotations.Schema;

namespace movie_service.Models;

public class Showtime
{
    public Guid Id { get; set; }

    [ForeignKey("Movie")]
    public Guid MovieId { get; set; }

    public Movie? Movie { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public decimal Price { get; set; }
}
