using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Bloggie.Web.Models.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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

            var emailConfig = _configuration.GetSection("EmailConfig");
            var username = emailConfig.GetValue<string>("Username");
            var password = emailConfig.GetValue<string>("Password");
            var host = emailConfig.GetValue<string>("Host");
            var port = emailConfig.GetValue<int>("Port");
            var fromEmail = emailConfig.GetValue<string>("FromEmail");

            var message = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = objEmailParameters.Subject,
                Body = objEmailParameters.EmailBodyMessage,
                IsBodyHtml = true
            };

            try
            {
                message.To.Add(new MailAddress(objEmailParameters.ToEmailAddress));

                using (var mailClient = new SmtpClient(host, port))
                {
                    mailClient.UseDefaultCredentials = false;
                    mailClient.Credentials = new System.Net.NetworkCredential(username, password);
                    mailClient.EnableSsl = true; // Enable SSL/TLS
                    mailClient.Timeout = 30000; // Set timeout to 30 seconds
                    mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                    _logger.LogInformation("Sending email...");

                    await mailClient.SendMailAsync(message);
                    _logger.LogInformation("Email sent successfully.");
                }
                ModelState.Clear();
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "SMTP Exception occurred while sending the email.");
                _logger.LogError($"SMTP Status Code: {ex.StatusCode}");
                _logger.LogError($"SMTP Inner Exception: {ex.InnerException?.Message}");
                return StatusCode(500, $"SMTP Exception: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while sending the email.");
                return StatusCode(500, $"Unexpected Error: {ex.Message}");
            }
            finally
            {
                // Explicitly dispose the message object to free resources
                message.Dispose();
            }

            return View("Index", new EmailEntity());
        }
    }
}
