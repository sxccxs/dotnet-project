using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Settings
{
    public class JsonDbSettings
    {
        // property name must be the same as model name: UserModel - UserDirectory
        public string UserDirectory { get; set; }
    }
}
