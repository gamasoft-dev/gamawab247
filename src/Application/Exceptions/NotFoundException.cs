using System;
namespace Application.Exceptions
{
    public class NotFoundException: Exception
    {
        public NotFoundException(string message): base(message)
        { }

        
        public NotFoundException(string message, object resourceId) : base(message)
        {
            ResourceId = resourceId;
        }

        public object ResourceId;
    }
}

