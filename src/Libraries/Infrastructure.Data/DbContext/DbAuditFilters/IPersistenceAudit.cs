using System;

namespace Infrastructure.Data.DbContext.DbAuditFilters
{
	public interface IPersistenceAudit
	{
		Guid? GetCreatedById { get; }
	}
}
