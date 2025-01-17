﻿using System;
namespace Domain.Exceptions
{
    public class InternalServerException : HttpException
	{
		public override string  Message {get;}
		public InternalServerException(string message): base(message)
		{
			Message = base.Message;
		}

        public InternalServerException(string message, int statusCode) : base(message, statusCode)
        {
        }
    }
}

