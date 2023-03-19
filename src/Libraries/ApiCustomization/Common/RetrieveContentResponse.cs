using System;
using Domain.Enums;

namespace ApiCustomization.Common
{
	public class RetrieveContentResponse
	{
		public bool IsSuccessful { get; set; }
		public string Response { get; set; }
		public ESessionState? UpdatedSessionState { get; set; }
		public bool DisContinueProcess { get; set; } = false;
	}
}

