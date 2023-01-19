using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class RequestDto
    {
        public Guid? BusinessId { get; set; }
        public Guid IndustryId { get; set; }
        public string Message { get; set; }
        public string MessageType { get; set; }
        public int Priority { get; set; }
        public List<RequestOption> RequestOption { get; set; }
    }

    public class RequestOptionDto
    {
        public Guid RequestId { get; set; }
        public string RequestKeyword { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class GetRequestDto
    {
        public Guid Id { get; set; }
        public Guid? BusinessId { get; set; }
        public Guid IndustryId { get; set; }
        public string Message { get; set; }
        public string MessageType { get; set; }
        public int Priority { get; set; }
        public List<RequestOption> RequestOption { get; set; }
    }
}