﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Abstractions.Interfaces.UserInterfaces
{
    public interface IHashingService
    {
        string Hash(string key);
    }
}
