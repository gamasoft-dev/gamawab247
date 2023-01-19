using System;
namespace Application.Exceptions
{
    public class FormInputValidationException: Exception
    {
        public object ErrorSource;
        public FormInputValidationException(string message, object source): base(message)
        {
            ErrorSource = source;
        }
    }
}

