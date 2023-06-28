using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;

namespace Application.DTOs
{
    public class BroadcastMessageDto : CreateBroadcastMessageDto
    {
        public Guid Id { get; set; }
        public string ErrorMessage { get; set; }
        public string Busisness { get; set; }
    }

    public class CreateBroadcastMessageDto
    {
        public string To { get; set; }
        public string From { get; set; }
        public string ApiKey { get; set; }
        public string Message { get; set; }
        public EBroadcastMessageStatus Status { get; set; }

    }

    public class UpdateBroadcastMessageDto : CreateBroadcastMessageDto
    {
    }
}
