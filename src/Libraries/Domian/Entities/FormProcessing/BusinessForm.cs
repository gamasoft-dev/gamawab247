using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;
using Domain.Entities.DialogMessageEntitties;
using Domain.Entities.FormProcessing.ValueObjects;
using Domain.Enums;

namespace Domain.Entities.FormProcessing
{
    /// <summary>
    /// This entity holds information about a form processing for a business
    /// </summary>
    public class BusinessForm: AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid? BusinessConversationId { get; set; }

        [Column(TypeName = "jsonb")]
        public List<FormElement> FormElements { get; set; }
        [Column(TypeName = "jsonb")]
        public List<KeyValueObj> Headers { get; set; }
        [Column(TypeName = "jsonb")]
        public List<FormResponseKvp> ResponseKvps { get; set; }

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

        public int Counter { get; set; }
        public bool IsRequestSuccessful { get; set; }
        public ICollection<UserFormData> UserFormData { get; set; }
        public Guid BusinessId { get; set; }
        public Business Business { get; set; }

        /// <summary>
        /// This is the business nessage that is sent after summary form message has been sent
        /// This is optional. especially if the process ends at the summary message.
        /// </summary>
        public Guid? ConclusionBusinessMessageId { get; set; }
        public BusinessMessage ConclusionBusinessMessage{ get; set; }
     
    }
}

