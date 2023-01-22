using Application.Helpers;
using Infrastructure.Data.DbContext.DbAuditFilters;
using System;

namespace Application.AuditServices
{
	public class PersistenceAuditService: IPersistenceAudit
	{
		public PersistenceAuditService()
		{ }

		private Guid? _createdBy;
		public Guid? GetCreatedById 
		{
			
			get { return _createdBy; } 
			
			set { _createdBy = WebHelper.UserId; } 
		}
	}
}
