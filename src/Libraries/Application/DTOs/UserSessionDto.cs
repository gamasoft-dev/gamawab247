using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class UserSessionDto
    {
        public string WaId { get; set; }
        public string SessionId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime ExpectedEndDateTime { get; set; }
        public SessionFlag SessionFlag { get; set; }
    }

    public enum SessionFlag
    {
        Initiated =1,
        Modified=2,
    }
}