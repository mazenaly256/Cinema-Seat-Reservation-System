using System.ComponentModel.DataAnnotations.Schema;

namespace movie_service.Models;

public class MovieGenre
{
    [ForeignKey("Movie")]
    public Guid MovieId { get; set; }

    public Movie? Movie { get; set; }


    [ForeignKey("Genre")]
    public Guid GenreId { get; set; }

    public Genre? Genre { get; set; }

}
