using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
 
using MimeKit;
using MailKit.Security;
using Resort.Application.Services;
 
namespace Resort.Infrastructure.Emails
{
    public class EmailService : IEmailService
    {
        private readonly string _sendGridKey;
        public EmailService(IConfiguration configuration)
        {
            _sendGridKey = configuration["SendGrid:Key"];
        }

        public async Task<bool> SendEmailsAsync(List<string> toSelectedEmails, string subject, string body)
        {

            bool isSentSuccessfullyFlag = true;
            string fromEmail = "eyad80701@gmail.com";
            string smtpHost = "smtp.gmail.com"; // Your SMTP server
            int smtpPort = 587;   //587 Default SMTP port (may vary)
            string smtpUsername = "eyad80701@gmail.com";
            //var smtpPassword = "ixFCIxx412_ixxx0";
            //string smtpPassword = "rvpa gahb fhdd qfvo";
            string smtpPassword = "zefr iojc ybjs sxfd";
            try
            {

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Resort Offers", fromEmail));
                foreach (string toEmail in toSelectedEmails)
                {

                    message.To.Add(new MailboxAddress("Our Dear Guest", toEmail));
                    message.Subject = subject;

                    message.Body = new TextPart("html")
                    {
                        Text = body,
                    };

                    using (var client = new MailKit.Net.Smtp.SmtpClient())
                    {
                        await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
                        await client.AuthenticateAsync(smtpUsername, smtpPassword);
                        await client.SendAsync(message);
                        await client.DisconnectAsync(true);
                    }
                }
            }
            catch (Exception ex)
            {
                isSentSuccessfullyFlag = false;
            }
            return isSentSuccessfullyFlag;

        }

        public async Task<bool> SendOneEmailAsync(string toEmail, string subject, string body)
        {

            bool isSentSuccessfullyFlag = true;
            string fromEmail = "eyad80701@gmail.com";
            string smtpHost = "smtp.gmail.com"; // Your SMTP server
            int smtpPort = 587;   //587 Default SMTP port (may vary)
            string smtpUsername = "eyad80701@gmail.com";
            //var smtpPassword = "ixFCIxx412_ixxx0";
            string smtpPassword = "rvpa gahb fhdd qfvo";
            try
            {

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Resort Management", fromEmail));
              
                message.To.Add(new MailboxAddress("Our Dear Guest", toEmail));
                message.Subject = subject;

                message.Body = new TextPart("html")
                {
                    Text = body,
                };

                    using (var client = new MailKit.Net.Smtp.SmtpClient())
                    {
                        await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
                        await client.AuthenticateAsync(smtpUsername, smtpPassword);
                        await client.SendAsync(message);
                        await client.DisconnectAsync(true);
                    }
                 
            }
            catch (Exception ex)
            {
                isSentSuccessfullyFlag = false;
            }
            return isSentSuccessfullyFlag;

        }

        // the first code by using System.Net.Mail package that almost built in or provided package by .net ; 
        //public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        //{
        //    bool isSentSuccessfullyFlag = true;
        //    var fromEmail = "eyad80701@gmail.com";
        //    var smtpHost = "smtp.gmail.com"; // Your SMTP server
        //    var smtpPort = 587;   //587 Default SMTP port (may vary)
        //    var smtpUsername = "eyad80701@gmail.com";
        //    //var smtpPassword = "ixFCIxx412_ixxx0";
        //    var smtpPassword = "rvpa gahb fhdd qfvo";

        //    using (var client = new SmtpClient(smtpHost))
        //    {
        //        client.Port = smtpPort;
        //        client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
        //        client.EnableSsl = true; // Use SSL for secure connection


        //        var mailMessage = new MailMessage(fromEmail, toEmail)
        //        {
        //            Subject = subject,
        //            Body = body,
        //            IsBodyHtml = true // If you want to send HTML emails, set this to true
        //        };

        //        try
        //        {
        //            //client.Send(mailMessage);
        //            await client.SendMailAsync(mailMessage);
        //            Console.WriteLine("Email sent successfully!");
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Error sending email: {ex.Message}");
        //            isSentSuccessfullyFlag = false;
        //        }
        //    }
        //    return isSentSuccessfullyFlag;
        //}




        // by using  SendGrid Package and using the key provided then key value configuration will be in appsetting 

        //public async Task<bool> SendEmailAsync(string email, string subject, string message)
        //{
        //    var client = new SendGridClient(_sendGridKey);
        //    var from = new EmailAddress("hello@dotnetmastery.com", "DotNetMastery - White Lagoon");
        //    var to = new EmailAddress(email);
        //    var msg = MailHelper.CreateSingleEmail(from, to, subject, "", message);
        //    var response = await client.SendEmailAsync(msg);

        //    if (response.StatusCode == System.Net.HttpStatusCode.Accepted || response.StatusCode == System.Net.HttpStatusCode.OK)
        //    {
        //        return true;
        //    }
        //    return false;
        //}
    }
}
