using System;
namespace Domain.Exceptions
{
	public class BgBusinessConfigException: BackgroundException
	{
		public int errorCode { get; set; }
		public BgBusinessConfigException(string message,
			Exception innerException = null): base(message, innerException)
		{
			errorCode = 500;
		}
	}

}

