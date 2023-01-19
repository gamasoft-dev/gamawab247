using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Domain.Entities.Identities;
using Infrastructure.Data.DbContext.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Infrastructure.Data.DbContext.DbAuditFilters;
using Domain.Entities.DialogMessageEntitties;
using Domain.Entities.FormProcessing;

namespace Infrastructure.Data.DbContext
{
    public class AppDbContext : IdentityDbContext<User, Role, Guid,
        UserClaim, UserRole, UserLogin, RoleClaim, UserToken>, IPersistenceAudit
    {
        public AppDbContext(DbContextOptions<AppDbContext> options, IPersistenceAudit persistenceAudit) : base(options)
        {
            GetCreatedById = persistenceAudit.GetCreatedById;
        }

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        /// <summary>
        /// This resolves to the currently logged in user.
        /// </summary>
        public Guid? GetCreatedById { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            // customize identity tables to utilize custom names
            UserConfiguration.ApplyUserIdentityConfigurations(builder);
            
            builder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);
        }

        /// <summary>
        /// This overrides the base SaveChanges Async to perform basic Auditing business logic
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            // Auditable details entity pre-processing
            Audit();

            return await base.SaveChangesAsync(cancellationToken);
        }
        
        private void Audit()
        {
            var entries = ChangeTracker.Entries().Where(x => x.Entity is IAuditableEntity
                                                             && (x.State == EntityState.Modified
                                                             || x.State == EntityState.Added));
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added )
                {
                    ((IAuditableEntity)entry.Entity).CreatedAt = DateTime.UtcNow;
                    ((IAuditableEntity)entry.Entity).CreatedById = GetCreatedById;
                }
                ((IAuditableEntity)entry.Entity).UpdatedAt = DateTime.UtcNow;
            }            
        }

        public override DbSet<User> Users { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<BusinessMessageSettings> BusinessMessageSettings { get; set; }
        public DbSet<Business> Businesses { get; set; }
        public DbSet<Industry> Industries { get; set; }
        public DbSet<SystemSettings> SystemSettings { get; set; }
        public DbSet<InboundMessage> InboundMessages { get; set; }
        public DbSet<OutboundMessage> OutboundMessages { get; set; }

        // new message implementations.
        public DbSet<BusinessMessage> BusinessMessages { get; set; }
        public DbSet<BusinessConversation> BusinessConversations { get; set; }
        public DbSet<ListMessage> ListMessages { get; set; }
        public DbSet<ReplyButtonMessage> ReplyButtonMessages { get; set; }
        public DbSet<TextMessage> TextMessages { get; set; }
        public DbSet<WhatsappUser> WhatsappUsers { get; set; }
        public DbSet<MessageLog> MessageLogs { get; set; }

        // form processing
        public DbSet<UserFormData> UserFormDatas { get; set; }
        public DbSet<BusinessForm> BusinessForms { get; set; }
        public DbSet<FormRequestResponse> FormRequestResponses { get; set; }
        public DbSet<BusinessFormConlusionConfig> BusinessFormConlusionConfigs { get; set; }
    }
}
