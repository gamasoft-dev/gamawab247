using BillProcessorAPI.Entities;
using BillProcessorAPI.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BillProcessorAPI.Data.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<BillTransaction>
    {
        public void Configure(EntityTypeBuilder<BillTransaction> builder)
        {
            builder.HasOne(x => x.BillPayerInfo)
                .WithMany()
                .HasForeignKey(x => x.BillPayerInfoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
