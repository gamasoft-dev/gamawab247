using Domain.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Domain.Entities
{
    public class Industry : AuditableEntity
    {
		public Industry()
		{
			Businesses = new Collection<Business>();
		}
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

		public ICollection<Business> Businesses { get; set; }
	}
}