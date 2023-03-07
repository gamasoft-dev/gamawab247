using System;
using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.FormProcessing
{
    public class FormRequestResponse: AuditableEntity
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
        public bool IsValidationResponse { get; set; }
        public string Direction { get; set; }
        public string FormElement { get; set; }
        public string MessageType { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }

        public Guid BusinessFormId { get; set; }
        public bool IsSummaryMessage { get; set; }
        public Guid BusinessId { get; set; }
        public BusinessForm BusinessForm { get; set; }
        public string FollowUpPartnerContentIntegrationKey { get; set; }

        //public override bool Equals(object obj)
        //{
        //    return obj is FormRequestResponse response &&
        //           Id.Equals(response.Id);
        //}

        //public override int GetHashCode()
        //{
        //    return HashCode.Combine(Id);
        //}
    }
}

