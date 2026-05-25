using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using movie_service.Models;

namespace movie_service.Data.Configurations;

public class ShowtimeConfiguration : IEntityTypeConfiguration<Showtime>
{
    public void Configure(EntityTypeBuilder<Showtime> builder)
    {
        builder.ToTable("Showtimes");

        builder.HasKey(st => st.Id);

        builder.Property(st => st.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(st => st.StartTime)
            .IsRequired();

        builder.Property(st => st.EndTime)
            .IsRequired();

        builder.Property(st => st.Price)
            .IsRequired()
            .HasPrecision(8, 2);

        builder.HasData(
            new Showtime
            {
                Id = new Guid("30000000-0000-0000-0000-000000000001"),
                MovieId = new Guid("10000000-0000-0000-0000-000000000001"),
                StartTime = new DateTime(2027, 4, 11, 16, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(2027, 4, 11, 19, 0, 0, DateTimeKind.Utc),
                Price = 30
            },

            new Showtime
            {
                Id = new Guid("30000000-0000-0000-0000-000000000002"),
                MovieId = new Guid("10000000-0000-0000-0000-000000000002"),
                StartTime = new DateTime(2027, 4, 11, 19, 30, 0, DateTimeKind.Utc),
                EndTime = new DateTime(2027, 4, 11, 21, 36, 0, DateTimeKind.Utc),
                Price = 35
            },

            new Showtime
            {
                Id = new Guid("30000000-0000-0000-0000-000000000003"),
                MovieId = new Guid("10000000-0000-0000-0000-000000000003"),
                StartTime = new DateTime(2027, 4, 12, 14, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(2027, 4, 12, 16, 28, 0, DateTimeKind.Utc),
                Price = 25
            },

            new Showtime
            {
                Id = new Guid("30000000-0000-0000-0000-000000000004"),
                MovieId = new Guid("10000000-0000-0000-0000-000000000004"),
                StartTime = new DateTime(2027, 4, 12, 17, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(2027, 4, 12, 19, 56, 0, DateTimeKind.Utc),
                Price = 45
            },

            new Showtime
            {
                Id = new Guid("30000000-0000-0000-0000-000000000005"),
                MovieId = new Guid("10000000-0000-0000-0000-000000000005"),
                StartTime = new DateTime(2026, 12, 13, 18, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(2026, 12, 13, 20, 49, 0, DateTimeKind.Utc),
                Price = 35
            },

            new Showtime
            {
                Id = new Guid("30000000-0000-0000-0000-000000000006"),
                MovieId = new Guid("10000000-0000-0000-0000-000000000001"),
                StartTime = new DateTime(2026, 5, 11, 7, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(2026, 5, 11, 11, 0, 0, DateTimeKind.Utc),
                Price = 40
            },

            new Showtime
            {
                Id = new Guid("30000000-0000-0000-0000-000000000007"),
                MovieId = new Guid("10000000-0000-0000-0000-000000000001"),
                StartTime = new DateTime(2027, 1, 12, 8, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(2027, 1, 12, 12, 0, 0, DateTimeKind.Utc),
                Price = 40
            },

            new Showtime
            {
                Id = new Guid("30000000-0000-0000-0000-000000000008"),
                MovieId = new Guid("10000000-0000-0000-0000-000000000002"),
                StartTime = new DateTime(2027, 1, 13, 12, 30, 0, DateTimeKind.Utc),
                EndTime = new DateTime(2027, 1, 13, 14, 36, 0, DateTimeKind.Utc),
                Price = 50
            },

            new Showtime
            {
                Id = new Guid("30000000-0000-0000-0000-000000000009"),
                MovieId = new Guid("10000000-0000-0000-0000-000000000003"),
                StartTime = new DateTime(2026, 5, 11, 13, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(2026, 5, 11, 15, 28, 0, DateTimeKind.Utc),
                Price = 45
            },

            new Showtime
            {
                Id = new Guid("30000000-0000-0000-0000-000000000010"),
                MovieId = new Guid("10000000-0000-0000-0000-000000000003"),
                StartTime = new DateTime(2026, 12, 11, 13, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(2026, 12, 11, 15, 28, 0, DateTimeKind.Utc),
                Price = 45
            },

            new Showtime
            {
                Id = new Guid("30000000-0000-0000-0000-000000000011"),
                MovieId = new Guid("10000000-0000-0000-0000-000000000005"),
                StartTime = new DateTime(2027, 12, 31, 7, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(2027, 12, 31, 11, 50, 0, DateTimeKind.Utc),
                Price = 35
            }
        );
    }
}
