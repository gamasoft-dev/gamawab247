using Domain.Common;
using Domain.Entities.Identities;
using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Business : AuditableEntity
    {
        public Guid Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public string Slogan { get; set; }
        public string RCNumber { get; set; }
        public string Address { get; set; }

        // Navigation properties
        public Guid IndustryId { get; set; }
        public Industry Industry { get; set; }

        public Guid? AdminUserId { get; set; }
        public User AdminUser { get; set; }

        public Guid? BusinessMessageSettingsId { get; set; }
        public BusinessMessageSettings BusinessMessageSettings { get; set; }

        public ICollection<User> Users { get; set; }
    }
}