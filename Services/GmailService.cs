using Microsoft.Extensions.Options;
using Models.DTO;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class GmailService: IMailService
    {
        private readonly MailSetting mailSetting;
        public GmailService(IOptions<MailSetting> option)
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
                    Host = mailSetting.SMTPHost,
                };
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(mailSetting.FromEmailAddress, mailSetting.FromEmailPassword);
                await client.SendMailAsync(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
