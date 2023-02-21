using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;
using Domain.Entities.FormProcessing.ValueObjects;

namespace Domain.Entities
{
	public class Partner: AuditableEntity
	{
		public Guid Id  { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }

		public Guid? BusinessId { get; set; }
	}

	public class PartnerIntegrationDetails
	{
		public Guid Id { get; set; }
		public string PartnerApiProcessIndentifier { get; set; }

		public List<FormHeader> Headers { get; set; }
        public List<FormHeader> Parameters { get; set; }

        public string FullUrl { get; set; }

		public Guid PartnerId { get; set; }
		public Partner Partner { get; set; }
	}
}

