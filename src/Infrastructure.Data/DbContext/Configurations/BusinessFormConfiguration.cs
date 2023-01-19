using System;
using Domain.Entities.FormProcessing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DbContext.Configurations
{
    public class BusinessFormConfiguration : IEntityTypeConfiguration<BusinessForm>
    {
        public void Configure(EntityTypeBuilder<BusinessForm> builder)
        {
            builder.HasOne(x => x.Business)
                .WithMany()
                .HasForeignKey(x => x.BusinessId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.ConclusionBusinessMessage)
                .WithMany()
                .HasForeignKey(x => x.ConclusionBusinessMessageId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

