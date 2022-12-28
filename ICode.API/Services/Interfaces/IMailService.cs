using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IMailService
    {
        Task SendMailAsync(string ToAddress, string subject, string body);
    }
}
