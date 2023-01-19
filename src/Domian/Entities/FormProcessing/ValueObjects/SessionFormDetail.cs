using Domain.Enums;
using System;
using System.Collections.Generic;

namespace Domain.Entities.FormProcessing.ValueObjects
{
    /// <summary>
    /// This is an object the holds current state values of the form fill process.
    /// </summary>
    public record SessionFormDetail
    {
        public SessionFormDetail()
        {
            UserData = new Dictionary<string, object>();
        }
        public Guid BusinessFormId { get; set; }
        public int CurrentElementId { get; set; }
        public string CurrentFormElement { get; set; }
        public string NextFormElement { get; set; }
        public bool IsValidationRequired { get; set; }
        public bool IsFormQuestionSent { get; set; }
        public string ValidationError { get; set; }
        public bool IsFormCompleted { get; set; }
        public int LastElementId { get; set; }

        public string ValidationProcessorKey { get; set; }
        public bool IsValueConfirmed { get; set; }
        public EDataType CurrentFormElementType { get; set; }

        /// <summary>
        /// This is a string of all request and response of the user in format
        /// Element1: UserData ,
        /// Element2: UserData2 etc
        /// </summary>
        public string Payload { get; set; }
        public BusinessForm BusinessForm { set; get; }
        public IDictionary<string, object> UserData { get; set; }
    }
}

