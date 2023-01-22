using System;
using Domain.Entities.FormProcessing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DbContext.Configurations
{
    public class FormRequestResponseConfiguration: IEntityTypeConfiguration<FormRequestResponse>
    {

        public void Configure(EntityTypeBuilder<FormRequestResponse> builder)
        {
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.To);
            builder.HasIndex(x => x.From);
            builder.HasIndex(x => x.CreatedAt);

            //builder.HasOne(x => x.BusinessForm)
            //    .WithMany()
            //    .HasForeignKey(x => x.BusinessFormId)
            //    .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

