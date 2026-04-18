using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using reservation_service.Models;

namespace reservation_service.Data.Configurations;

public class SeatHoldConfiguration : IEntityTypeConfiguration<SeatHold>
{
    public void Configure(EntityTypeBuilder<SeatHold> builder)
    {
        builder.ToTable("SeatHolds");

        builder.ToTable(t => t.HasCheckConstraint("CK_SeatNumber_Format", "SeatNumber LIKE '[A-D][1-4]'"));

        builder.HasKey(r => new { r.ShowtimeId, r.SeatNumber });

        builder.Property(m => m.SeatNumber)
            .IsRequired()
            .HasMaxLength(2);

        builder.Property(m => m.HeldUntil)
            .IsRequired();
    }
}
