using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using reservation_service.Models;

namespace reservation_service.Data.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("Reservations");

        builder.ToTable(t => t.HasCheckConstraint("CK_SeatNumber_Format", "\"SeatNumber\" ~ '^[A-D][1-4]$'"));

        builder.HasKey(r => new { r.ShowtimeId, r.SeatNumber});

        builder.Property(m => m.SeatNumber)
            .IsRequired()
            .HasMaxLength(2);
    }
}
