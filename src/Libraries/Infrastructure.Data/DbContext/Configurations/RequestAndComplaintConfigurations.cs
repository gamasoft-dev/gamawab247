using System;
using Domain.Entities;
using Domain.Entities.RequestAndComplaints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DbContext.Configurations
{
	public class RequestAndComplaintConfigurations: IEntityTypeConfiguration<RequestAndComplaint>
    {

        public void Configure(EntityTypeBuilder<RequestAndComplaint> builder)
        {
            builder.Property(x => x.CustomerId).IsRequired();
            builder.Property(x => x.CustomerName);


            builder.HasOne(x => x.TreatedBy)
                .WithMany()
                .HasForeignKey(x => x.TreatedById)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(x => x.CustomerId);
            builder.HasIndex(x => x.TicketId);
            builder.HasIndex(x => x.ResolutionDate);
            builder.HasIndex(x => x.CreatedAt);

        }
    }
}

