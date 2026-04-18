using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using reservation_service.Models;

namespace reservation_service.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(m => m.PaidAmount)
            .IsRequired();

        builder.Property(m => m.PaidAt)
            .IsRequired();

        builder.HasOne(p => p.Reservation)
            .WithOne(r => r.Payment)
            .HasForeignKey<Reservation>(r => r.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
