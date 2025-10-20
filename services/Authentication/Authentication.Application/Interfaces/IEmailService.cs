using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Application.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string recipientEmail, string recipientName, string templateFileName, Dictionary<string, string> templateData);
    }
}
