using Domain.Entities.FormProcessing.ValueObjects;
using Domain.Enums;
using System;
using System.Collections.Generic;

namespace Application.DTOs.BusinessDtos
{
    public class BusinessFormDto
    {
        public Guid Id { get; set; }
        public EUrlMethodType UrlMethodType { get; set; }
        public string SubmissionUrl { get; set; }
        public Guid BusinessId { get; set; }
        public Guid? BusinessConversationId { get; set; }
        /// <summary>
        /// MessageType = Text, interactive{Button, list}
        /// </summary>
        /// <example>Button</example>
        public EMessageType MessageType { get; set; }
        public List<CreateBusinessFormFormElementDto> FormElements { get; set; }
        public List<BusinessFormHeaderDto> AuthHeaderElement { get; set; }
        public List<BusinessFormResponseDto> ResponseProperty { get; set; }
    }

    public class CreateBusinessFormDto
    {
        public EUrlMethodType UrlMethodType { get; set; } = EUrlMethodType.POST;
        public string SubmissionUrl { get; set; }
        public Guid BusinessId { get; set; }
        public Guid? BusinessConversationId { get; set; }

        /// <summary>
        /// MessageType = Text, interactive{Button, list}
        /// </summary>
        /// <example>Button</example>
        public EMessageType MessageType { get; set; } = EMessageType.Text;

        public List<FormElement> FormElements { get; set; }
    }

    public class BusinessFormHeaderDto: KeyValueObj
    {
    }

    public class BusinessFormResponseDto: FormResponseKvp
    {
    }

    public class CreateBusinessFormFormElementDto
    {
        public string Key { get; set; }
        public EDataType KeyDataType { get; set; }
        public bool IsValidationRequired { get; set; }
        public string ValidationProcessorKey { get; set; }
        public string Label { get; set; }
    }


    public class UpdateBusinessFormDto
    {
        public Guid Id { get; set; }
        public EUrlMethodType UrlMethodType { get; set; }
        public string SubmissionUrl { get; set; }
        public Guid BusinessId { get; set; }
        public Guid? BusinessConversationId { get; set; }
        /// <summary>
        /// MessageType = Text, interactive{Button, list}
        /// </summary>
        /// <example>Button</example>
        public EMessageType MessageType { get; set; }
        public List<UpdateFormElementDto> FormProperties { get; set; }
        public List<BusinessFormHeaderDto> AuthHeaderElement { get; set; }
        public List<BusinessFormResponseDto> ResponseProperty { get; set; }
    }

    public class UpdateFormElementDto:FormElement
    {
    }


}