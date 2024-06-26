using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Bloggie.Web.Models.ViewModel;

namespace Bloggie.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
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
                var user = new IdentityUser
                {
                    UserName = registerViewModel.Username,
                    Email = registerViewModel.Email
                };

                var result = await _userManager.CreateAsync(user, registerViewModel.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");

                    if (!_signInManager.IsSignedIn(User) && User.IsInRole("SuperAdmin"))
                    {
                        await SendRegistrationEmailAsync(registerViewModel.Email);
                    }

                    TempData["Message"] = "🎉 Successfully registered! Welcome aboard! 🚀";
                    TempData["MessageType"] = "success";
                    return RedirectToAction("Login");
                }
                foreach (var error in result.Errors)
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
                var emailConfig = _configuration.GetSection("EmailConfig").GetSection("Smtp");
                var username = emailConfig["Username"];
                var password = emailConfig["Password"];
                var host = emailConfig["Host"];
                var port = emailConfig.GetValue<int>("Port");
                var fromEmail = emailConfig["FromEmail"];

                var message = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = "Regiatration Suuccessfully to the  Bloogie!",
                    Body = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        body {{
            font-family: 'Arial', sans-serif;
            color: #333;
            line-height: 1.6;
            background-color: #f4f4f9;
            padding: 20px;
        }}
        .container {{
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #ffffff;
            border-radius: 10px;
            box-shadow: 0 4px 8px rgba(0,0,0,0.1);
        }}
        .header {{
            text-align: center;
            background-color: #007BFF;
            padding: 10px;
            border-radius: 10px 10px 0 0;
            color: #fff;
        }}
        .header h1 {{
            margin: 0;
            font-size: 24px;
        }}
        .content {{
            padding: 20px;
            background-color: #ffffff;
        }}
        .content p {{
            font-size: 16px;
            color: #555;
        }}
        .content ul {{
            padding-left: 20px;
        }}
        .content ul li {{
            margin-bottom: 10px;
        }}
        .content ul li strong {{
            color: #007BFF;
        }}
        .footer {{
            text-align: center;
            margin-top: 20px;
            color: #777;
        }}
        .footer a {{
            color: #007BFF;
            text-decoration: none;
        }}
        .footer a:hover {{
            text-decoration: underline;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Welcome to Bloogie!</h1>
        </div>
        <div class='content'>
            <p>Dear {userEmail},</p>
            <p>Welcome to Bloogie! 🎉</p>
            <p>Thank you for registering with us. We're thrilled to have you as part of our community. At Bloogie, we believe in the power of words and the magic of storytelling. Whether you're here to share your thoughts, discover new ideas, or connect with like-minded individuals, you've come to the right place.</p>
            <p>Here's what you can look forward to:</p>
            <ul>
                <li><strong>Inspiring Content:</strong> Dive into a world of engaging articles, insightful opinions, and creative pieces.</li>
                <li><strong>Community Connection:</strong> Join discussions, interact with authors, and make new friends.</li>
                <li><strong>Personalized Experience:</strong> Customize your feed to match your interests and preferences.</li>
            </ul>
            <p>To get started, simply log in to your account and explore all that Bloogie has to offer. If you have any questions or need assistance, our support team is here to help.</p>
            <p>Happy blogging!</p>
            <p>Warm regards,</p>
            <p>The Bloogie Team</p>
        </div>
        <div class='footer'>
            <p>Stay connected with us on social media:</p>
            <p>
                <a href='#'>Facebook</a> | 
                <a href='#'>Twitter</a> | 
                <a href='#'>Instagram</a>
            </p>
        </div>
    </div>
</body>
</html>
",
                    IsBodyHtml = true
                };


                message.To.Add(new MailAddress(userEmail));

                using (var client = new SmtpClient(host, port))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential(username, password);
                    client.EnableSsl = true;
                    client.Timeout = 30000;

                    await client.SendMailAsync(message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending registration email to {userEmail}");
                // Handle exception as needed
            }
        }


        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(loginViewModel.Username, loginViewModel.Password, false, lockoutOnFailure: false);
                if (result.Succeeded)
                {

                    if (!_signInManager.IsSignedIn(User) && User.IsInRole("SuperAdmin"))
                    {
                        await SendLoginEmailAsync(loginViewModel.Username);
                    }

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(loginViewModel);
                }
            }

            return View(loginViewModel);
        }

        private async Task SendLoginEmailAsync(string username)
        {
            try
            {
                var emailConfig = _configuration.GetSection("EmailConfig").GetSection("Smtp");
                var fromEmail = emailConfig["FromEmail"];
                var usernameEmail = emailConfig["Username"];
                var passwordEmail = emailConfig["Password"];
                var hostEmail = emailConfig["Host"];
                var portEmail = emailConfig.GetValue<int>("Port");

                var message = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = "Login Successfully to the Bloogie!",
                    Body = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        body {{
            font-family: Arial, sans-serif;
            color: #333;
            line-height: 1.6;
        }}
        .container {{
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #f9f9f9;
            border-radius: 10px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }}
        .header {{
            text-align: center;
        }}
        .header h1 {{
            color: #007BFF;
        }}
        .content {{
            padding: 20px;
            background-color: #fff;
            border-radius: 10px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }}
        .footer {{
            text-align: center;
            margin-top: 20px;
            color: #777;
        }}
        .footer a {{
            color: #007BFF;
            text-decoration: none;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Welcome Back to Bloogie!</h1>
        </div>
        <div class='content'>
            <p>Dear {username},</p>
            <p>We're delighted to see you back on Bloogie! 🎉</p>
            <p>Whether you're here to continue your blogging journey, catch up on the latest articles, or connect with your favorite authors, we're excited to have you back.</p>
            <p>Here's a reminder of what you can do:</p>
            <ul>
                <li><strong>Explore New Content:</strong> Discover fresh and inspiring articles from a variety of topics.</li>
                <li><strong>Reconnect with the Community:</strong> Engage with other members, comment on posts, and join discussions.</li>
                <li><strong>Personalize Your Experience:</strong> Adjust your settings to see the content that matters most to you.</li>
            </ul>
            <p>If you have any questions or need assistance, our support team is always here to help.</p>
            <p>Happy blogging!</p>
            <p>Warm regards,</p>
            <p>The Bloogie Team</p>
        </div>
        <div class='footer'>
            <p>Stay connected with us on social media: 
                <a href='#'>Facebook</a> | 
                <a href='#'>Twitter</a> | 
                <a href='#'>Instagram</a>
            </p>
        </div>
    </div>
</body>
</html>
",
                    IsBodyHtml = true
                };


                message.To.Add(new MailAddress(fromEmail)); // Send notification to the user's own email

                using (var client = new SmtpClient(hostEmail, portEmail))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential(usernameEmail, passwordEmail);
                    client.EnableSsl = true;
                    client.Timeout = 30000;

                    await client.SendMailAsync(message);
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
