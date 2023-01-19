using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
	public enum EResponseType
	{
		Custom = 1,
		Default = 2
	}

	public enum EMessagePosition: int
    {
		Top = 1,
		Middle = 2,
		Last = 3,
		FallBack = 4
    }
}
