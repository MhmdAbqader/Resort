using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resort.Application.Services
{
    public interface IEmailService 
    {
        Task<bool> SendOneEmailAsync(string toEmail, string subject, string body);
        Task<bool> SendEmailsAsync(List<string> toSelectedEmails, string subject, string body);
    }
   
}
