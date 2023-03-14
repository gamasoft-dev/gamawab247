using System;
using Domain.Entities.Identities;
using Domain.Enums;
using System.Collections.Generic;

namespace Application.DTOs
{
	public class RequestAndComplaintDto
	{
        public Guid Id { get; set; }
        /// <summary>
        /// This is either user whatsapp username or phonenumber, instgram username etc
        /// </summary>
        public string CustomerId { get; set; }

        public string Subject { get; set; }

        public string Channel { get; set; }

        public string Detail { get; set; }

        public List<string> Responses { get; set; }

        /// <summary>
        /// This is the complaint or request unique identifier. This is generated for every ticker raised
        /// </summary>
        public string TicketId { get; set; }

        public Guid BusinessId { get; set; }

        public ERequestComplaintType Type { get; set; }

        public string CallBackUrl { get; set; }

        public DateTime? ResolutionDate { get; set; }

        internal string ResolutionStatus { get; set; }

        public Guid? TreatedById { get; set; }

        public string TreatedBy { get; set; }

    }

    public class CreateRequestAndComplaintDto {

        public CreateRequestAndComplaintDto()
        {
            ResolutionStatus = EResolutionStatus.Pending.ToString();
        }

        /// <summary>
        /// This is either user whatsapp username or phonenumber, instgram username etc
        /// </summary>
        public string CustomerId { get; set; }

        public string Subject { get; set; }

        public string Channel { get; set; }

        public string Detail { get; set; }

        /// <summary>
        /// This is the complaint or request unique identifier. This is generated for every ticker raised
        /// </summary>
        internal string TicketId { get; set; }

        public Guid BusinessId { get; set; }

        public ERequestComplaintType Type { get; set; }

        public string CallBackUrl { get; set; }

        public string ResolutionStatus { get; set; }

        public Guid? TreatedById { get; set; }

    }

    public class UpdateRequestAndComplaintDto: CreateRequestAndComplaintDto
    {
        public List<string> Response { get; set; }
    }
}

