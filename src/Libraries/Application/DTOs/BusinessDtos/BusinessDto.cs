using Domain.Common;
using System;

namespace Application.DTOs.BusinessDtos
{
    public class CreateBusinessDto 
    {
        public string Name { get; set; }
        public Guid IndustryId { get; set; }
        public string AvatarUrl { get; set; }
        public string PhoneNumber { get; set; }


        public string AdminFirstName { get; set; }
        public string AdminLastName { get; set; }
        public string BusinessAdminEmail { get; set; }
        public string AdminPhoneNumber { get; set; }
    }

    public class CreateBusinessSetupDto 
    {
        internal string WebhookUrl { get; set; }
        public string ApiKey { get; set; }
        public int TestCounter { get; set; } = 3;
        public Guid BusinessId { get; set; }
        public string BotName { get; set; }
        public string BotDescription { get; set; }
    }
    
    public class BusinessDto : AuditableEntity
    {
        public Guid Id { get; set; }
        public string PhoneNumber { get; set; }
        public string BusinessAdminEmail { get; set; }
        public string Name { get; set; }
        public string AvatarUrl { get; set; }

        // Navigation properties
        public Guid IndustryId { get; set; }
        public string IndustryName { get; set; }

        public Guid?  AdminId { get; set; }
        public string AdminFirstName { get; set; }
        public string AdminEmail { get; set; }
        public string AdminSurName { get; set; }
        public string AdminPhoneNumber { get; set; }
    }
}
