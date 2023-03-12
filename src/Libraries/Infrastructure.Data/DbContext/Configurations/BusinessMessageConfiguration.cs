using Domain.Entities;
using Domain.Entities.DialogMessageEntitties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DbContext.Configurations
{
    public class BusinessMessageConfiguration: IEntityTypeConfiguration<BusinessMessage>
    {
        public void Configure(EntityTypeBuilder<BusinessMessage> builder)
        {
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Position).IsRequired();
            
            builder.HasIndex(x => x.Position);
            builder.HasIndex(x => x.MessageType);
            builder.HasIndex(x => x.Name);

            builder.HasIndex(x => new {x.BusinessId , x.Position });

            builder.HasOne(x => x.BusinessForm)
                .WithMany()
                .HasForeignKey(x => x.BusinessFormId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }

    public class OutboundMessageConfiguration : IEntityTypeConfiguration<OutboundMessage>
    {
        public void Configure(EntityTypeBuilder<OutboundMessage> builder)
        {
            builder.HasIndex(x => x.From);
            builder.HasIndex(x => x.Type);
            builder.HasIndex(x => x.RecipientWhatsappId);
            builder.HasIndex(x => x.CreatedAt);
            builder.HasIndex(x => x.BusinessId);

            builder.HasIndex(x => new { x.WhatsAppMessageId, x.RecipientWhatsappId, x.CreatedAt });

        }
    }

    public class InboundMessageConfiguration : IEntityTypeConfiguration<InboundMessage>
    {
        public void Configure(EntityTypeBuilder<InboundMessage> builder)
        {
            builder.HasIndex(x => x.Wa_Id);
            builder.HasIndex(x => x.MsgOptionId);
            builder.HasIndex(x => x.ContextMessageId);
            builder.HasIndex(x => x.BusinessIdMessageId);

            builder.HasIndex(x => new { x.ResponseProcessingStatus, x.CreatedAt});
        }
    }
}
