using System;

namespace Application.DTOs.Common
{
	public class AuditableDto
	{
		public DateTime? CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
		public Guid? CreatedById { get; set; }
	}

	public interface IAuditableDto
	{
		public DateTime? CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
		public Guid? CreatedById { get; set; }
	}
}
