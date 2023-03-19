using System;
using Domain.Entities.FormProcessing.ValueObjects;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;

namespace Domain.Entities.RequestAndComplaints
{
	public class RequestAndComplaintConfig: AuditableEntity
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

        public string FullUrl { get; set; }
        public string MetaData { get; set; }
        public Guid? BusinessId { get; set; }

		public string WebHookUrl { get; set; }
		public bool RequireWebHookNotification { get; set; }

		public string SubjectKey { get; set; } = "subject";
		public string DetailKey { get; set; } = "detail";

		public int TimeInHoursOfRequestResolution { get; set; }
		public int TimeInHoursOfComplaintResolution { get; set; }
	}
}

