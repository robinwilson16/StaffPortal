using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using StaffPortal.Models;

namespace StaffPortal.Utilities
{
    public class EmailSender : IEmailSender
    {
        public EmailSender(IOptions<EmailSettings> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public EmailSettings Options { get; } //set only via Secret Manager

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(Options.Host, Options.Port, Options.UseSSL, Options.Username, Options.Password, subject, message, email);
        }

        public Task Execute(string host, int port, bool useSSL, string username, string password, string subject, string message, string email)
        {
            //For Office 365 Multi-Factor Authentication must be disabled - if not then this error appears (app passwords do not work):
            //5.7.57 SMTP; Client was not authenticated to send anonymous mail during MAIL FROM
            SmtpClient client = new SmtpClient(host)
            {
                Port = port,
                UseDefaultCredentials = false,
                EnableSsl = true,
                Credentials = new NetworkCredential(username, password)
            };

            MailMessage emailMessage = new MailMessage();
            emailMessage.From = new MailAddress(Options.FromAddress);
            emailMessage.To.Add(email);
            emailMessage.Subject = subject;

            //if (EmailCC != null && EmailCC != "") { emailMessage.Bcc.Add(new MailAddress(EmailCC)); }
            //if (EmailBCC != null && EmailBCC != "") { emailMessage.Bcc.Add(new MailAddress(EmailBCC)); }

            emailMessage.IsBodyHtml = true;

            string html = message;

            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(html, new ContentType("text/html"));

            emailMessage.AlternateViews.Add(htmlView);

            return client.SendMailAsync(emailMessage);
        }
    }
}
