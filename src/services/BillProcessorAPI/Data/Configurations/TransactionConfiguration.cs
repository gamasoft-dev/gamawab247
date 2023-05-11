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
            builder.HasIndex(x => x.UpdatedAt);
            builder.HasIndex(x => x.BillNumber);
            builder.HasIndex(x => x.Status);
        }
    }

    public class BillPayerInfoConfiguration : IEntityTypeConfiguration<BillPayerInfo>
    {
        public void Configure(EntityTypeBuilder<BillPayerInfo> builder)
        {
            builder.HasIndex(x => x.UpdatedAt);
            builder.HasIndex(x => x.billCode);
        }
    }
}
