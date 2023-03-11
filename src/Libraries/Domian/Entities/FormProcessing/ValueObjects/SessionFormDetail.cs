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
            UserData = new Dictionary<string, string>();
        }
        public Guid BusinessFormId { get; set; }

        public FormElement NextFormElement { get; set; }

        public FormElement CurrentFormElement { get; set; }

        public bool IsFormQuestionSent { get; set; }

        public bool IsFormResponseRecieved { get; set; }

        public string CurrentValidationError { get; set; }

        public bool IsCurrentValueConfirmed { get; set; }

        public bool IsFormCompleted { get; set; }

        /// <summary>
        /// This is a string of all request and response of the user in format
        /// Element1: UserData ,
        /// Element2: UserData2 etc
        /// </summary>
        public string Payload { get; set; }

        public BusinessFormVM BusinessForm { set; get; }

        public IDictionary<string, string> UserData { get; set; }
    }
}

