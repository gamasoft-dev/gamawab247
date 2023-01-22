using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DbContext.Configurations;

public class MessageLogConfiguration: IEntityTypeConfiguration<MessageLog>
{
    public void Configure(EntityTypeBuilder<MessageLog> builder)
    {
        builder.Property(x => x.WhatsappUserId).IsRequired();
        builder.Property(x => x.RequestResponseData).IsRequired();
            
        builder.HasIndex(x => x.MessageDirection);
        builder.HasIndex(x => x.To);
        builder.HasIndex(x => x.From);
        builder.HasIndex(x => x.CreatedAt);

        builder.HasOne(x => x.WhatsappUser)
            .WithMany(x => x.MessageLogs)
            .HasForeignKey(x => x.WhatsappUserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}