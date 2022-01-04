﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class UserModel : BaseModel
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string HashedPassword { get; set; }

        public UserModel(string userName, string email, string hashedPassword)
        {
            UserName = userName;
            Email = email;
            HashedPassword = hashedPassword;
        }

    }
}
