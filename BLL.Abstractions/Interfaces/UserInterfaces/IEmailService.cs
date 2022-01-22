using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Abstractions.Interfaces.UserInterfaces
{
    public interface IEmailService
    {
        void SendEmail(string toEmail, string emailSubject, string emailHtmlBody);
    }
}
