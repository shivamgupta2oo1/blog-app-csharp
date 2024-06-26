using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Net.Pop3;
using MailKit.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Bloggie.Web.Models.ViewModel;
using Bloggie.Web.Models.Domain;
using Bloggie.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Controllers
{
    public class SmtpEmailController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmtpEmailController> _logger;
        private readonly BloggieDbContext _dbContext;

        public SmtpEmailController(
            IConfiguration configuration,
            ILogger<SmtpEmailController> logger,
            BloggieDbContext dbContext)
        {
            _configuration = configuration;
            _logger = logger;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var emails = _dbContext.ReceivedEmails.OrderByDescending(e => e.Date).ToList();
            ViewBag.Emails = emails;
            return View(new EmailEntity());
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail(EmailEntity objEmailParameters)
        {
            if (objEmailParameters == null)
            {
                _logger.LogError("Invalid email parameters.");
                return BadRequest("Invalid email parameters.");
            }

            var smtpConfig = _configuration.GetSection("EmailConfig:Smtp");
            var username = smtpConfig.GetValue<string>("Username");
            var password = smtpConfig.GetValue<string>("Password");
            var host = smtpConfig.GetValue<string>("Host");
            var port = smtpConfig.GetValue<int>("Port");
            var fromEmail = smtpConfig.GetValue<string>("FromEmail");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("", fromEmail));
            message.To.Add(new MailboxAddress("", objEmailParameters.ToEmailAddress));
            message.Subject = objEmailParameters.Subject;

            var body = new TextPart("html")
            {
                Text = objEmailParameters.EmailBodyMessage
            };

            message.Body = body;

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

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RetrieveEmails()
        {
            var pop3Config = _configuration.GetSection("EmailConfig:Pop3");
            var username = pop3Config.GetValue<string>("Username");
            var password = pop3Config.GetValue<string>("Password");
            var host = pop3Config.GetValue<string>("Host");
            var port = pop3Config.GetValue<int>("Port");

            try
            {
                using (var client = new Pop3Client())
                {
                    await client.ConnectAsync(host, port, SecureSocketOptions.SslOnConnect);
                    await client.AuthenticateAsync(username, password);

                    var emails = new List<ReceivedEmails>();
                    for (int i = 0; i < client.Count; i++)
                    {
                        var message = await client.GetMessageAsync(i);

                        var mainContent = GetMainContentFromEmail(message.HtmlBody ?? message.TextBody);

                        // Check if the email with this subject and date already exists
                        var existingEmail = await _dbContext.ReceivedEmails.FirstOrDefaultAsync(e =>
                            e.Subject == message.Subject && e.Date == message.Date.UtcDateTime);

                        if (existingEmail == null)
                        {
                            var receivedEmail = new ReceivedEmails
                            {
                                Id = Guid.NewGuid(), // Ensure this matches your PostgreSQL UUID setup
                                From = message.From.ToString(),
                                Subject = message.Subject,
                                Body = mainContent,
                                Date = message.Date.UtcDateTime,
                            };

                            // Save to database using Entity Framework Core
                            _dbContext.ReceivedEmails.Add(receivedEmail);
                            await _dbContext.SaveChangesAsync();

                            emails.Add(receivedEmail);
                        }
                    }

                    ViewBag.Emails = emails;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving emails.");
                return StatusCode(500, $"Unexpected Error: {ex.Message}");
            }

            return RedirectToAction("Index");
        }

        private string GetMainContentFromEmail(string emailBody)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(emailBody);
            var mainContentDiv = doc.DocumentNode.SelectSingleNode("//div[@dir='auto']");

            return mainContentDiv != null ? mainContentDiv.InnerText.Trim() : string.Empty;
        }
    }
}
