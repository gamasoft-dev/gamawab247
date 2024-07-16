using System;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.Identities
{
	public class Role : IdentityRole<Guid>
	{
		public Role() : base ()
		{ 
			Id  = Guid.NewGuid();
		}
        // add extra special column/properties here
    }

}
