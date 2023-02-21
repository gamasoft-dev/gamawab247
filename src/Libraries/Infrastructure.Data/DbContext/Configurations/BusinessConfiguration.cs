using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DbContext.Configurations
{
    public class BusinessConfiguration: IEntityTypeConfiguration<Business>
    {
        public void Configure(EntityTypeBuilder<Business> builder)
        {
            builder.HasOne(x => x.Industry)
                .WithMany(y => y.Businesses)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Property(x => x.Name).IsRequired();

            builder.HasOne(x => x.AdminUser)
                .WithMany()
                .HasForeignKey(x=>x.AdminUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.BusinessMessageSettings)
               .WithOne(x=>x.Business)
               .HasForeignKey<BusinessMessageSettings>(x => x.BusinessId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.Email).IsUnique();
            builder.HasIndex(x => x.Name).IsUnique();
            builder.HasIndex(x => x.PhoneNumber).IsUnique();
        }
    }
    public class BusinessMessageConfig : IEntityTypeConfiguration<BusinessMessageSettings>
    {
        public void Configure(EntityTypeBuilder<BusinessMessageSettings> builder)
        {
            builder.HasIndex(x => x.ApiKey);
        }
    }

    public class IndustryConfiguration : IEntityTypeConfiguration<Industry>
    {
        public void Configure(EntityTypeBuilder<Industry> builder)
        {
            builder.HasMany(x => x.Businesses)
                .WithOne(x=>x.Industry)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Property(x => x.Name).IsRequired();

            builder.HasIndex(x => x.Name).IsUnique();
        }
    }

}
