using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Domain.Common;

namespace Domain.Entities
{
    public class Partner: AuditableEntity
	{
		public Partner()
		{
			PartnerIntegrationDetails = new List<PartnerIntegrationDetails>();
		}
		public Guid Id  { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }

		public Guid? BusinessId { get; set; }

		public ICollection<PartnerIntegrationDetails> PartnerIntegrationDetails { get; set; }
	}
}

