using System;
using Domain.Enums;

namespace Application.Exceptions
{
    public class FormBgProcessorException: Exception
    {
        public string FormProcessingStatus;
        public string ErrorForUser;
        public ESessionState? NewSessionState;
        public FormBgProcessorException(string message, string formProcessingStatus, string errorForUser = null, ESessionState? newSessionState = null) : base(message)
        {
            FormProcessingStatus = formProcessingStatus;
            ErrorForUser = errorForUser;
            NewSessionState = newSessionState;
        }

        public FormBgProcessorException(string message, string errorForUser = null, ESessionState? newSessionState = null) : base(message)
        {
            this.ErrorForUser = errorForUser;
            this.NewSessionState = newSessionState;
        }

        public FormBgProcessorException(string message, Exception innerException) : base(message, innerException)
        {
           
        }
    }
}

