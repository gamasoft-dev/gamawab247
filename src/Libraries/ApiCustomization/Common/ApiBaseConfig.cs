using System;
namespace ApiCustomization.Common
{
	public abstract class ApiBaseConfig
	{
		public string BaseUrl { get; set; }
		public int Timeout { get; set; }
	}
}

