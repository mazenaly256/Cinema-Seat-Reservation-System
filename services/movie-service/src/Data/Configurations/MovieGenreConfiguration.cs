using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using movie_service.Models;

namespace movie_service.Data.Configurations;

public class MovieGenreConfiguration : IEntityTypeConfiguration<MovieGenre>
{
    public void Configure(EntityTypeBuilder<MovieGenre> builder)
    {
        builder.ToTable("MovieGenres");

        builder.HasKey(mg => new { mg.MovieId, mg.GenreId });

        builder.HasData(
            new MovieGenre
            {
                MovieId = new Guid("10000000-0000-0000-0000-000000000001"),
                GenreId = new Guid("20000000-0000-0000-0000-000000000003")
            },

            new MovieGenre
            {
                MovieId = new Guid("10000000-0000-0000-0000-000000000001"),
                GenreId = new Guid("20000000-0000-0000-0000-000000000005")
            },

            new MovieGenre
            {
                MovieId = new Guid("10000000-0000-0000-0000-000000000002"),
                GenreId = new Guid("20000000-0000-0000-0000-000000000001")
            },

            new MovieGenre
            {
                MovieId = new Guid("10000000-0000-0000-0000-000000000002"),
                GenreId = new Guid("20000000-0000-0000-0000-000000000002")
            },

            new MovieGenre
            {
                MovieId = new Guid("10000000-0000-0000-0000-000000000003"),
                GenreId = new Guid("20000000-0000-0000-0000-000000000002")
            },

            new MovieGenre
            {
                MovieId = new Guid("10000000-0000-0000-0000-000000000003"),
                GenreId = new Guid("20000000-0000-0000-0000-000000000005")
            },

            new MovieGenre
            {
                MovieId = new Guid("10000000-0000-0000-0000-000000000004"),
                GenreId = new Guid("20000000-0000-0000-0000-000000000004")
            },

            new MovieGenre
            {
                MovieId = new Guid("10000000-0000-0000-0000-000000000004"),
                GenreId = new Guid("20000000-0000-0000-0000-000000000005")
            },

            new MovieGenre
            {
                MovieId = new Guid("10000000-0000-0000-0000-000000000005"),
                GenreId = new Guid("20000000-0000-0000-0000-000000000001")
            },

            new MovieGenre
            {
                MovieId = new Guid("10000000-0000-0000-0000-000000000005"),
                GenreId = new Guid("20000000-0000-0000-0000-000000000003")
            }
        );
    }
}
