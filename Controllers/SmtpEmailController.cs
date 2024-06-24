// using System;
// using System.Net.Mail;
// using System.Threading.Tasks;
// using Bloggie.Web.Models.ViewModel;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.Logging;

// namespace Bloggie.Web.Controllers
// {
//     public class SmtpEmailController : Controller
//     {
//         private readonly IConfiguration _configuration;
//         private readonly ILogger<SmtpEmailController> _logger;

//         public SmtpEmailController(IConfiguration configuration, ILogger<SmtpEmailController> logger)
//         {
//             _configuration = configuration;
//             _logger = logger;
//         }

//         public IActionResult Index()
//         {
//             return View();
//         }

//         [HttpPost]
//         public async Task<IActionResult> SendEmail(EmailEntity objEmailParameters)
//         {
//             if (objEmailParameters == null)
//             {
//                 _logger.LogError("Invalid email parameters or file.");
//                 return BadRequest("Invalid email parameters or file.");
//             }

//             var emailConfig = _configuration.GetSection("EmailConfig");
//             var username = emailConfig.GetValue<string>("Username");
//             var password = emailConfig.GetValue<string>("Password");
//             var host = emailConfig.GetValue<string>("Host");
//             var port = emailConfig.GetValue<int>("Port");
//             var fromEmail = emailConfig.GetValue<string>("FromEmail");

//             var message = new MailMessage
//             {
//                 From = new MailAddress(fromEmail),
//                 Subject = objEmailParameters.Subject,
//                 Body = objEmailParameters.EmailBodyMessage,
//                 IsBodyHtml = true
//             };

//             try
//             {
//                 message.To.Add(new MailAddress(objEmailParameters.ToEmailAddress));

//                 using (var mailClient = new SmtpClient(host, port))
//                 {
//                     mailClient.UseDefaultCredentials = false;
//                     mailClient.Credentials = new System.Net.NetworkCredential(username, password);
//                     mailClient.EnableSsl = true; // Enable SSL/TLS
//                     mailClient.Timeout = 30000; // Set timeout to 30 seconds
//                     mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;

//                     _logger.LogInformation("Sending email...");

//                     await mailClient.SendMailAsync(message);
//                     _logger.LogInformation("Email sent successfully.");

//                     // Set ViewBag.EmailSentSuccess to true to trigger success message in view
//                     ViewBag.EmailSentSuccess = true;
//                 }
//                 ModelState.Clear();
//             }
//             catch (SmtpException ex)
//             {
//                 _logger.LogError(ex, "SMTP Exception occurred while sending the email.");
//                 _logger.LogError($"SMTP Status Code: {ex.StatusCode}");
//                 _logger.LogError($"SMTP Inner Exception: {ex.InnerException?.Message}");
//                 return StatusCode(500, $"SMTP Exception: {ex.Message}");
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError(ex, "An unexpected error occurred while sending the email.");
//                 return StatusCode(500, $"Unexpected Error: {ex.Message}");
//             }
//             finally
//             {
//                 // Explicitly dispose the message object to free resources
//                 message.Dispose();
//             }

//             return View("Index", new EmailEntity());
//         }
//     }
// }





using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Net.Pop3;
using MailKit.Security;
using Bloggie.Web.Models.ViewModel;

namespace Bloggie.Web.Controllers
{
    public class SmtpEmailController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmtpEmailController> _logger;

        public SmtpEmailController(IConfiguration configuration, ILogger<SmtpEmailController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail(EmailEntity objEmailParameters)
        {
            if (objEmailParameters == null)
            {
                _logger.LogError("Invalid email parameters or file.");
                return BadRequest("Invalid email parameters or file.");
            }

            var smtpConfig = _configuration.GetSection("EmailConfig:Smtp");
            var username = smtpConfig.GetValue<string>("Username");
            var password = smtpConfig.GetValue<string>("Password");
            var host = smtpConfig.GetValue<string>("Host");
            var port = smtpConfig.GetValue<int>("Port");
            var fromEmail = smtpConfig.GetValue<string>("FromEmail");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("",fromEmail));
            message.To.Add(new MailboxAddress("", objEmailParameters.ToEmailAddress));
            message.Subject = objEmailParameters.Subject;

            var body = new BodyBuilder();
            body.HtmlBody = objEmailParameters.EmailBodyMessage;
            message.Body = body.ToMessageBody();

            try
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(username, password);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                _logger.LogInformation("Email sent successfully.");
                ViewBag.EmailSentSuccess = true;
                ModelState.Clear();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while sending the email.");
                ViewBag.EmailSentSuccess = false;
                return StatusCode(500, $"Unexpected Error: {ex.Message}");
            }
            finally
            {
                message.Dispose();
            }

            return View("Index", new EmailEntity());
        }

        public async Task<IActionResult> RetrieveEmails()
        {
            var pop3Config = _configuration.GetSection("EmailConfig:Pop3");
            var username = pop3Config.GetValue<string>("Username");
            var password = pop3Config.GetValue<string>("Password");
            var host = pop3Config.GetValue<string>("Host");
            var port = pop3Config.GetValue<int>("Port");

            using (var client = new Pop3Client())
            {
                try
                {
                    await client.ConnectAsync(host, port, SecureSocketOptions.SslOnConnect);
                    await client.AuthenticateAsync(username, password);

                    var emails = new List<EmailMessageViewModel>();
                    for (int i = 0; i < client.Count; i++)
                    {
                        var message = await client.GetMessageAsync(i);
                        emails.Add(new EmailMessageViewModel
                        {
                            From = message.From.ToString(),
                            Subject = message.Subject,
                            Date = message.Date.DateTime.ToString("yyyy-MM-dd HH:mm:ss")
                        });
                    }

                    await client.DisconnectAsync(true);

                    ViewBag.Emails = emails;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An unexpected error occurred while retrieving emails.");
                    return StatusCode(500, $"Unexpected Error: {ex.Message}");
                }
            }

            return View("Index", new EmailEntity());
        }
    }

    internal class EmailMessageViewModel
    {
        public string From { get; set; }
        public string Subject { get; set; }
        public string Date { get; set; }
    }
}
