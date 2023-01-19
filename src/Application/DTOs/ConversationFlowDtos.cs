using Application.DTOs.Common;
using System;

namespace Application.DTOs
{
	#region industry conversation flow dtos
	public class CreateIndustryConversationFlowDto
	{
		public string RequestKeywords { get; set; }

		public string RequestResponse { get; set; }
		public string SubjectMatter { get; set; }
		public string ResponseImageUrl { get; set; }

		public bool ForceCreate { get; set; }
		public string Description { get; set; }
	}

	public class UpdateIndustryConversationFlowDto : CreateIndustryConversationFlowDto
	{ }

	public class GetIndustryConversationFlowDto : AuditableDto
	{
		public Guid Id { get; set; }

		/// <summary>
		/// This is a stringified list of keywords seperated by commas. not more than 3 words.
		/// </summary>
		public string RequestKeywords { get; set; }

		public string Response { get; set; }
		public string SubjectMatter { get; set; }
		public string ResponseImageUrl { get; set; }
		public string Description { get; set; }
	}

	#endregion

	#region business conversation flow
	public class CreateBusinessConversationFlowDto : CreateIndustryConversationFlowDto
	{
		internal Guid BusinessId { get; set; }
	}

	public class UpdateBusinessConversationFlowDto: CreateBusinessConversationFlowDto
	{ }

	public class GetBusinessConversationFlowDto: GetIndustryConversationFlowDto
	{
		public Guid BusinessId { get; set; }
	}

	#endregion
}
