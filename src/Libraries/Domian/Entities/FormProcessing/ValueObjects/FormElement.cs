using Domain.Enums;
using System;
namespace Domain.Entities.FormProcessing.ValueObjects
{
    /// <summary>
    /// This is for the form 
    /// </summary>
    public class FormElement
    {
        public int Id { get; set; }
        public string Key { get; set; }
        /// <summary>
        /// This relates to the EKeyDataType enum
        /// </summary>
        public EDataType KeyDataType { get; set; }
        public bool IsValidationRequired { get; set; }
        /// <summary>
        /// Name/Indentifier of the processor responsible for validating this form key/element
        /// </summary>
        public string ValidationProcessorKey { get; set; }
        public string Label { get; set; } = "Enter Bill Code";

        /// <summary>
        /// This denotes that the next response after this input will be generated or retrieved from
        /// an integration or local implementation.
        /// </summary>
        public Guid? NextResponsePartnerContentIntegrationId { get; set; }
    }
}