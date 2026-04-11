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
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(st => st.StartTime)
            .IsRequired();

        builder.Property(st => st.EndTime)
            .IsRequired();

        builder.HasData(
            new Showtime
            {
                Id = new Guid("30000000-0000-0000-0000-000000000001"),
                MovieId = new Guid("10000000-0000-0000-0000-000000000001"),
                StartTime = new DateTime(2026, 4, 11, 18, 0, 0),
                EndTime = new DateTime(2026, 4, 11, 21, 0, 0)
            },

            new Showtime
            {
                Id = new Guid("30000000-0000-0000-0000-000000000002"),
                MovieId = new Guid("10000000-0000-0000-0000-000000000002"),
                StartTime = new DateTime(2026, 4, 11, 21, 30, 0),
                EndTime = new DateTime(2026, 4, 11, 23, 36, 0)
            },

            new Showtime
            {
                Id = new Guid("30000000-0000-0000-0000-000000000003"),
                MovieId = new Guid("10000000-0000-0000-0000-000000000003"),
                StartTime = new DateTime(2026, 4, 12, 16, 0, 0),
                EndTime = new DateTime(2026, 4, 12, 18, 28, 0)
            },

            new Showtime
            {
                Id = new Guid("30000000-0000-0000-0000-000000000004"),
                MovieId = new Guid("10000000-0000-0000-0000-000000000004"),
                StartTime = new DateTime(2026, 4, 12, 19, 0, 0),
                EndTime = new DateTime(2026, 4, 12, 21, 56, 0)
            },

            new Showtime
            {
                Id = new Guid("30000000-0000-0000-0000-000000000005"),
                MovieId = new Guid("10000000-0000-0000-0000-000000000005"),
                StartTime = new DateTime(2026, 4, 13, 20, 0, 0),
                EndTime = new DateTime(2026, 4, 13, 22, 49, 0)
            },

            new Showtime
            {
                Id = new Guid("30000000-0000-0000-0000-000000000006"),
                MovieId = new Guid("10000000-0000-0000-0000-000000000001"),
                StartTime = new DateTime(2026, 4, 11, 9, 0, 0),
                EndTime = new DateTime(2026, 4, 11, 13, 0, 0)
            },

            new Showtime
            {
                Id = new Guid("30000000-0000-0000-0000-000000000007"),
                MovieId = new Guid("10000000-0000-0000-0000-000000000001"),
                StartTime = new DateTime(2026, 4, 12, 10, 0, 0),
                EndTime = new DateTime(2026, 4, 12, 14, 0, 0)
            },

            new Showtime
            {
                Id = new Guid("30000000-0000-0000-0000-000000000008"),
                MovieId = new Guid("10000000-0000-0000-0000-000000000002"),
                StartTime = new DateTime(2026, 4, 13, 14, 30, 0),
                EndTime = new DateTime(2026, 4, 13, 16, 36, 0)
            },

            new Showtime
            {
                Id = new Guid("30000000-0000-0000-0000-000000000009"),
                MovieId = new Guid("10000000-0000-0000-0000-000000000003"),
                StartTime = new DateTime(2026, 4, 11, 15, 0, 0),
                EndTime = new DateTime(2026, 4, 11, 17, 28, 0)
            },

            new Showtime
            {
                Id = new Guid("30000000-0000-0000-0000-000000000010"),
                MovieId = new Guid("10000000-0000-0000-0000-000000000005"),
                StartTime = new DateTime(2026, 4, 13, 9, 0, 0),
                EndTime = new DateTime(2026, 4, 13, 13, 49, 0)
            }
        );
    }
}
