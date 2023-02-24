using Application.Helpers;
using Infrastructure.Data.DbContext.DbAuditFilters;
using System;

namespace Application.AuditServices
{
	public class PersistenceAuditService: IPersistenceAudit
	{
		public PersistenceAuditService()
		{
			_createdBy = WebHelper.UserId; 
        }

		private Guid? _createdBy;
		public Guid? GetCreatedById 
		{
			
			get { return _createdBy; } 
			 
		}
	}
}
