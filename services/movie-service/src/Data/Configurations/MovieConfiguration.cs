using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using movie_service.Models;

namespace movie_service.Data.Configurations;

public class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.ToTable("Movies");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(m => m.DurationMinutes)
            .IsRequired();


        builder.HasMany(m => m.Showtimes)
            .WithOne(st => st.Movie)
            .HasForeignKey(st => st.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(m => m.Genres)
            .WithOne(mg => mg.Movie)
            .HasForeignKey(mg => mg.MovieId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.HasData(
            new Movie
            {
                Id = new Guid("10000000-0000-0000-0000-000000000001"),
                Name = "Oppenheimer",
                DurationMinutes = 180
            },

            new Movie
            {
                Id = new Guid("10000000-0000-0000-0000-000000000002"),
                Name = "Dune: Part Two",
                DurationMinutes = 166
            },

            new Movie
            {
                Id = new Guid("10000000-0000-0000-0000-000000000003"),
                Name = "Spider-Man: No Way Home",
                DurationMinutes = 148
            },

            new Movie
            {
                Id = new Guid("10000000-0000-0000-0000-000000000004"),
                Name = "The Batman",
                DurationMinutes = 176
            },

            new Movie
            {
                Id = new Guid("10000000-0000-0000-0000-000000000005"),
                Name = "Interstellar",
                DurationMinutes = 169
            }
        );
    }
}
