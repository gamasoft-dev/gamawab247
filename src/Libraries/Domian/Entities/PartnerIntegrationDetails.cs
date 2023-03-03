using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;
using Domain.Entities.FormProcessing.ValueObjects;

namespace Domain.Entities
{
    public class PartnerIntegrationDetails: AuditableEntity
	{
		public Guid Id { get; set; }
		public string PartnerContentProcessorKey { get; set; }

        [Column(TypeName = "jsonb")]
        public List<KeyValueObj> Headers { get; set; }

        /// <summary>
        /// This is the key of the form element that may be used to retrieved the cached argument value that
        /// would be used for this response retrieval
        /// </summary>
        [Column(TypeName = "jsonb")]
        public List<KeyValueObj> Parameters { get; set; }

        [Column(TypeName = "jsonb")]
        public List<KeyValueObj> Configs { get; set; }

		public string FullUrl { get; set; }
		public string MetaData { get; set; }

		public Guid PartnerId { get; set; }
		public Partner Partner { get; set; }
	}
}

