using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.Identities
{
    public class User: IdentityUser<Guid>, IAuditableEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Verified { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Status { get; set; }
        public ICollection<UserActivity> UserActivities { get; set; }
		public Guid? CreatedById { get; set; }
		public Guid? BusinessId { get; set; }
		public Business Business { get; set; }
	}
}
