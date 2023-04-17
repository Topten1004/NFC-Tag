using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BqsClinoTag.Settings
{
    public class MailSettings
    {
        public string Mail { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Smtp { get; set; }
        public int PortSmtp { get; set; }
        public string Pop { get; set; }
        public int PortPop { get; set; }
        public string Bcc { get; set; }
    }
}
