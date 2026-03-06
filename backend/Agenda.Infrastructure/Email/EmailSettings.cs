using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Infrastructure.Email
{
    public class EmailSettings
    {
        public string Host { get; set; } = "mailpit";
        public int Port { get; set; } = 1025;
        public bool UseSsl { get; set; } = false;
        public string? User { get; set; }
        public string? Pass { get; set; }
    }
}
