using System;
namespace Domain.Entities.RequestAndComplaints
{
	public class RequestAndComplaintConfig: PartnerIntegrationDetails
	{
		public Guid? BusinessId { get; set; }
		public string WebHookUrl { get; set; }
		public bool RequireWebHookNotification { get; set; }

		public string SubjectKey { get; set; } = "subject";
		public string DetailKey { get; set; } = "detail";

		public int TimeInHoursOfRequestResolution { get; set; }
		public int TimeInHoursOfComplaintResolution { get; set; }
	}
}

