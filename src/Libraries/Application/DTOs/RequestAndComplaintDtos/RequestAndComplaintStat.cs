using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.DTOs.RequestAndComplaintDtos
{
    public class RequestAndComplaintStat
    {
        public Guid Id { get; set; }
        public string ResolutionStatus { get; set; }
        public ERequestComplaintType Type { get; set; }
    }
}
