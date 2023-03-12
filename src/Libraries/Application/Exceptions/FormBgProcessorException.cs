using System;
namespace Application.Exceptions
{
    public class FormBgProcessorException: Exception
    {
        public string FormProcessingStatus;
        public string ErrorForUser;
        public FormBgProcessorException(string message, string formProcessingStatus, string errorForUser = null): base(message)
        {
            FormProcessingStatus = formProcessingStatus;
            ErrorForUser = errorForUser;
        }

        public FormBgProcessorException(string message, string errorForUser = null) : base(message)
        {
            this.ErrorForUser = errorForUser;
        }

        public FormBgProcessorException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

