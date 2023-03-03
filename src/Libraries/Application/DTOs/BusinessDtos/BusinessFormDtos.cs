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
        public List<CreateBusinessFormFormElementDto> FormProperty { get; set; }
        public List<BusinessFormHeaderDto> AuthHeaderElement { get; set; }
        public List<BusinessFormResponseDto> ResponseProperty { get; set; }
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
        public List<UpdateFormElementDto> FormProperty { get; set; }
        public List<BusinessFormHeaderDto> AuthHeaderElement { get; set; }
        public List<BusinessFormResponseDto> ResponseProperty { get; set; }
    }

    public class UpdateFormElementDto:FormElement
    {
    }


}