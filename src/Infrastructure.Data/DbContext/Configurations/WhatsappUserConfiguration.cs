using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DbContext.Configurations
{
    public class WhatsappUserConfiguration : IEntityTypeConfiguration<WhatsappUser>
    {
        public void Configure(EntityTypeBuilder<WhatsappUser> builder)
        {
            builder.Property(x => x.WaId).IsRequired();
            builder.Property(x => x.PhoneNumber).IsRequired();

            builder.HasMany(x => x.MessageLogs)
                .WithOne(x=>x.WhatsappUser)
                .HasForeignKey(x => x.WhatsappUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.WaId).IsUnique();
            builder.HasIndex(x => x.PhoneNumber).IsUnique();
        }
    }
}
