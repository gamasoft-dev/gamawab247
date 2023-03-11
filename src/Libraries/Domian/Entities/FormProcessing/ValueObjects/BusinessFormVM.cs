using System;
using Domain.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.FormProcessing.ValueObjects
{
	public class BusinessFormVM
	{
        public Guid Id { get; set; }
        public Guid? BusinessConversationId { get; set; }

        [Column(TypeName = "jsonb")]
        public List<FormElement> FormElements { get; set; }
        [Column(TypeName = "jsonb")]
        public List<KeyValueObj> Headers { get; set; }

        // Use this to determine how and where to submission this form when collated completely.
        public bool IsFormToBeSubmittedToUrl { get; set; }

        /// <summary>
        /// Request url for respective business who wants/tends to make an http request to an external resource..
        /// </summary>
        /// <example>https://www.google.com/user/1/book-flight</example>
        public string SubmissionUrl { get; set; }
        /// <summary>
        /// Request type
        /// </summary>
        /// <example>POST</example>
        public int UrlMethodType { get; set; } = (int)EUrlMethodType.POST;

        public bool IsSummaryOfFormMessagesRequired { get; set; }

        public Guid BusinessId { get; set; }
        public Guid? ConclusionBusinessMessageId { get; set; }

        public BusinessFormVM MapBusinessFormToVM(BusinessForm model) {
            return new BusinessFormVM
            {
                Id = model.Id,
                BusinessConversationId = model.BusinessConversationId,
                FormElements = model.FormElements,
                Headers = model.Headers,
                IsFormToBeSubmittedToUrl = model.IsFormToBeSubmittedToUrl,
                SubmissionUrl = model.SubmissionUrl,
                UrlMethodType = model.UrlMethodType,
                IsSummaryOfFormMessagesRequired = model.IsSummaryOfFormMessagesRequired,
                BusinessId = model.BusinessId,
                ConclusionBusinessMessageId = model.ConclusionBusinessMessageId
            };
        }
    }
}

