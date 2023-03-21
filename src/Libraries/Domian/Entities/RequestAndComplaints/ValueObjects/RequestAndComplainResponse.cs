using System;
using System.Collections.Generic;

namespace Domain.Entities.RequestAndComplaints
{
    public class RequestAndComplainResponse
    {
        public string Response { get; set; }
        public DateTime? DateResponded { get; set; }
        public Guid RespondedById { get; set; }
        public String RespondedBy { get; set; }
    }

    public class RequestAndComplaintResponsList
    {
        public List<RequestAndComplainResponse> Responses { get; set; }
    }
}
