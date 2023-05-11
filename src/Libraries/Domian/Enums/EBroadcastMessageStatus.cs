using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum EBroadcastMessageStatus
    {
        Pending = 1,
        Processing,
        Sent,
        Failed
    }
}
