using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using movie_service.Models;

namespace movie_service.Data.Configurations;

public class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.ToTable("Genres");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");


        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(30);

        builder.HasMany(g => g.Movies)
            .WithOne(mg => mg.Genre)
            .HasForeignKey(mg => mg.GenreId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new Genre
            {
                Id = new Guid("20000000-0000-0000-0000-000000000001"),
                Name = "Sci-Fi"
            },

            new Genre
            {
                Id = new Guid("20000000-0000-0000-0000-000000000002"),
                Name = "Action"
            },

            new Genre
            {
                Id = new Guid("20000000-0000-0000-0000-000000000003"),
                Name = "Drama"
            },

            new Genre
            {
                Id = new Guid("20000000-0000-0000-0000-000000000004"),
                Name = "Crime"
            },

            new Genre
            {
                Id = new Guid("20000000-0000-0000-0000-000000000005"),
                Name = "Thriller"
            }
        );
    }
}
