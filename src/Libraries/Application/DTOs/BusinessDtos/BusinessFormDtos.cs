using Domain.Entities;
using Domain.Entities.DialogMessageEntitties;
using Domain.Entities.FormProcessing;
using Domain.Entities.FormProcessing.ValueObjects;
using Domain.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application.DTOs.BusinessDtos
{
    public class BusinessFormDto : CreateBusinessFormDto
    {
        public Guid Id { get; set; }
    }

    public class CreateBusinessFormDto
    {
        public EUrlMethodType UrlMethodType { get; set; }

        public string SubmissionUrl { get; set; }

        public Guid BusinessId { get; set; }

        public Guid? BusinessConversationId { get; set; }

        /// <summary>
        /// MessageType = Text, interactive {Button, list}
        /// </summary>
        /// <example>Button</example>
        public EMessageType MessageType { get; set; }

        public List<FormElement> FormElements { get; set; }

        public List<KeyValueObj> Headers { get; set; }

        public bool IsSummaryOfFormMessagesRequired { get; set; }

        /// <summary>
        /// This is the business nessage that is sent after the last form element
        /// This is optional. especially if the process ends at the summary message
        /// or last form element.
        /// </summary>
        public Guid? ConclusionBusinessMessageId { get; set; }
    }

    public class UpdateBusinessFormDto : CreateBusinessFormDto
    {
        internal Guid Id { get; set; }
    }
}