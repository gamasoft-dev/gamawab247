using Domain.Enums;
using System;
using System.Collections.Generic;

namespace Domain.Entities.FormProcessing.ValueObjects
{
    /// <summary>
    /// This is for the form 
    /// </summary>
    public class FormElement : IEquatable<FormElement>
    {
        public int Position { get; set; }
        public string Key { get; set; }
        /// <summary>
        /// This relates to the EKeyDataType enum
        /// </summary>
        public EDataType KeyDataType { get; set; } = EDataType.String;

        public bool IsValidationRequired { get; set; }

        /// <summary>
        /// Name/Indentifier of the processor responsible for validating this form key/element
        /// </summary>
        public string ValidationProcessorKey { get; set; }

        /// <summary>
        /// Thiis more so the body of the message that would be sent to the user
        /// e.g "Enter first name"
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// This should be set to true if the value of this formelement i.e its label
        /// would be retrieved from an external function or integration
        /// Note: If this value is true, then label should be empty and the {PartnerContentProcessorKey} would also have a value
        /// </summary>
        public bool ShouldRetrieveContentExternally { get; set; }

        /// <summary>
        /// This denotes that the next response after this input will be generated or retrieved from
        /// an integration or local implementation using.
        /// </summary>
        public string PartnerContentProcessorKey { get; set; }

        /// <summary>
        /// Set to true if this element is the last element of the form.
        /// </summary>
        public bool IsLastFormElement { get; set; } = false;

        public Guid? FollowUpMessageId { get; set; }

        public bool RequireUserInputResponse { get; set; }

        public int NextFormElementPosition { get; set; } 

        #region Get and Equal Impl

        public override bool Equals(object obj)
        {
            return Equals(obj as FormElement);
        }

        public bool Equals(FormElement other)
        {
            return other is not null &&
                   Position == other.Position ||
                   Key == other.Key;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Position, Key);
        }

        public static bool operator ==(FormElement left, FormElement right)
        {
            return EqualityComparer<FormElement>.Default.Equals(left, right);
        }

        public static bool operator !=(FormElement left, FormElement right)
        {
            return !(left == right);
        }

        #endregion
    }
}