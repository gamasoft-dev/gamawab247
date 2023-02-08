using System;
namespace Application.Exceptions
{
    public class ProcessCancellationException: Exception
    {
        public int Code { get; set; }
        public ProcessCancellationException(string message) : base(message)
        { }
        public ProcessCancellationException(string message, int code): base(message)
        {
            Code = code;
        }
    }

    public class FormConfigurationException : ProcessCancellationException
    {
        static string defaultMessage = $"An issue has occurred whilst processing your request," +
            $" Kindly contact your business admin {Environment.NewLine} {Environment.NewLine}" +
            $"You can send the keyword 'end' to end this dialog session and start afressh";

        public bool SendPromptMessageToUser { get; set; }
        public FormConfigurationException(string message, bool sendErrorPromptWithThisMessage): base(message)
        {
            SendPromptMessageToUser = sendErrorPromptWithThisMessage;
        }
        public FormConfigurationException(bool sendErrorPromptWithDefaultMessage) : base(defaultMessage)
        {
            SendPromptMessageToUser = sendErrorPromptWithDefaultMessage;
        }
    }
}

