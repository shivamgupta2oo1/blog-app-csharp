using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Bloggie.Web.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Bloggie.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
            IConfiguration configuration,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                var identityUser = new IdentityUser
                {
                    UserName = registerViewModel.Username,
                    Email = registerViewModel.Email
                };

                var identityResult = await _userManager.CreateAsync(identityUser, registerViewModel.Password);

                if (identityResult.Succeeded)
                {
                    var roleIdentityResult = await _userManager.AddToRoleAsync(identityUser, "User");

                    if (roleIdentityResult.Succeeded)
                    {
                        await SendRegistrationEmailAsync(registerViewModel.Email);

                        TempData["Message"] = "🎉 Successfully registered! Welcome aboard! 🚀";
                        TempData["MessageType"] = "success";
                        return RedirectToAction("Login");
                    }
                }

                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            TempData["Message"] = "⛔️ Registration error: Invalid ID or password. Please try again. 🔑";
            TempData["MessageType"] = "error";
            return View(registerViewModel);
        }

        private async Task SendRegistrationEmailAsync(string userEmail)
        {
            try
            {
                var emailConfig = _configuration.GetSection("EmailConfig");
                var username = emailConfig.GetValue<string>("Username");
                var password = emailConfig.GetValue<string>("Password");
                var host = emailConfig.GetValue<string>("Host");
                var port = emailConfig.GetValue<int>("Port");
                var fromEmail = emailConfig.GetValue<string>("FromEmail");

                var message = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = "Welcome to Bloggie!",
                    Body = $"Dear {userEmail},<br><br>Thank you for registering with Bloggie.",
                    IsBodyHtml = true
                };

                message.To.Add(new MailAddress(userEmail));

                using (var mailClient = new SmtpClient(host, port))
                {
                    mailClient.UseDefaultCredentials = false;
                    mailClient.Credentials = new System.Net.NetworkCredential(username, password);
                    mailClient.EnableSsl = true; // Enable SSL/TLS
                    mailClient.Timeout = 30000; // Set timeout to 30 seconds
                    mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                    await mailClient.SendMailAsync(message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending registration email to {userEmail}");
                // Handle exception as needed
            }
        }

        [HttpGet]
        public IActionResult Login(string ReturnUrl)
        {
            var model = new LoginViewModel
            {
                ReturnUrl = ReturnUrl,
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(loginViewModel.Username, loginViewModel.Password, false, false);

                if (signInResult != null && signInResult.Succeeded)
                {
                    await SendLoginEmailAsync(loginViewModel.Username);

                    if (!string.IsNullOrWhiteSpace(loginViewModel.ReturnUrl))
                    {
                        return Redirect(loginViewModel.ReturnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Message = "🔑 Your password is ❌";
                    ViewBag.MessageType = "error"; // Types: success, error, warning, info, question
                }

            }
            return View();
        }

        private async Task SendLoginEmailAsync(string username)
        {
            try
            {
                var emailConfig = _configuration.GetSection("EmailConfig");
                var fromEmail = emailConfig.GetValue<string>("FromEmail");

                var message = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = "Login Notification",
                    Body = $"Dear {username},<br><br>You have successfully logged in to Bloggie.",
                    IsBodyHtml = true
                };

                message.To.Add(new MailAddress(fromEmail)); // Send notification to the user's own email

                using (var mailClient = new SmtpClient())
                {
                    mailClient.UseDefaultCredentials = false;
                    mailClient.Credentials = new System.Net.NetworkCredential(fromEmail, emailConfig.GetValue<string>("Password"));
                    mailClient.Host = emailConfig.GetValue<string>("Host");
                    mailClient.Port = emailConfig.GetValue<int>("Port");
                    mailClient.EnableSsl = true; // Enable SSL/TLS

                    await mailClient.SendMailAsync(message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending login email notification for user {username}");
                // Handle exception as needed
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
