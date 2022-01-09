using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Settings
{
    public class EmailSettings
    {
        public string EmailAccountLogin { get; set; }

        public string EmailAccountPassword { get; set; }

        public string EmailHost { get; set; }

        public int EmailPort { get; set; }
    }
}
