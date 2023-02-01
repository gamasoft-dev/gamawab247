using Application.DTOs.CreateDialogDtos;
using Domain.Entities.DialogMessageEntitties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.BusinessDtos
{
    public class BusinessConversationDto
    {
		public Guid Id { get; set; }
		public Guid BusinessId { get; set; }
		public string Title { get; set; }
		public virtual ICollection<BusinessMessage> BusinessMessages { get; set; }
	}

	public class CreateBusinessConversationDtoo
	{
		public Guid BusinessId { get; set; }
		public string Title { get; set; }
		//public BusinessMessage BusinessMessages { get; set; }
	}

	public class UpdateBusinessConversationDto : CreateBusinessConversationDtoo
	{
		public Guid Id { get; set;}
	}
}
