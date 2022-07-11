using API.Models.DTO;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace API.Helper
{
    public interface IMail
    {
        Task SendMailAsync(string ToAddress, string subject, string body);
    }
    public class Mail : IMail
    {
        private readonly MailSetting mailSetting;
        public Mail(IOptions<MailSetting> option)
        {
            mailSetting = option.Value;
        }    
        public async Task SendMailAsync(string ToAddress, string subject, string body)
        {
            try
            {
                // MailMessage message = new MailMessage(new MailAddress(fromEmailAddress, fromEmailDisplayName), new MailAddress(toEmailAddress));
                MailMessage message = new MailMessage
                {
                    From = new MailAddress(mailSetting.FromEmailAddress, mailSetting.FromEmailDisplayName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                message.To.Add(new MailAddress(ToAddress));

                SmtpClient client = new SmtpClient
                {
                    Port = 587,
                    EnableSsl = true,
                    Credentials = new NetworkCredential(mailSetting.FromEmailAddress, mailSetting.FromEmailPassword),
                    Host = mailSetting.SMTPHost,
                };
                await client.SendMailAsync(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
