using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace BqsClinoTag.Grool
{
    public static class Droits
    {
        public enum Roles
        {
            API,
            SUPERADMIN,
            ADMIN,
            AGENT,
            MANAGER
        }
    }
}
