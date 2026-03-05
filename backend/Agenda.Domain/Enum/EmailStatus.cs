using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Domain.Enum
{
    public enum EmailStatus
    {
        Pending = 0,
        Sending = 1,
        Sent = 2,
        Error = 3,
    }
}
