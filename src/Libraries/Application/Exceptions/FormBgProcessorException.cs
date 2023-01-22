using System;
namespace Application.Exceptions
{
    public class FormBgProcessorException: Exception
    {
        public string FormProcessingStatus;
        public FormBgProcessorException(string message, string formProcessingStatus): base(message)
        {
            FormProcessingStatus = formProcessingStatus;
        }

        public FormBgProcessorException(string message) : base(message)
        { }

        public FormBgProcessorException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}

