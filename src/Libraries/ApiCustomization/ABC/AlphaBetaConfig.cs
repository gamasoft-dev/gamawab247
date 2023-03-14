using System;
using ApiCustomization.Common;

namespace ApiCustomization.ABC
{
	public class AlphaBetaConfig: ApiBaseConfig
	{
		public string HolderVerificationEndpoint { get; set; }
		public string ClientKey { get; set; }
        public string LinkGeneratorUserParamKey { get; set; }
		public string BillCodePaymentPageLink { get; set; }
		public bool IsMockRequest { get; set; }
		//public string  MyProperty { get; set; }
	}
}

