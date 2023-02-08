using BillProcessorAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BillProcessorAPI.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<BillPayerInfo>
    {
        public void Configure(EntityTypeBuilder<BillPayerInfo> builder)
        {
        }
    }
}
