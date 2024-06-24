using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModel;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Bloggie.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostLikeController : ControllerBase
    {
        private readonly IBlogPostLikeRepository blogPostLikeRepository;
        private readonly IUserRepository userRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BlogPostLikeController> _logger;

        public BlogPostLikeController(IBlogPostLikeRepository blogPostLikeRepository, IUserRepository userRepository, IConfiguration configuration, ILogger<BlogPostLikeController> logger)
        {
            this.blogPostLikeRepository = blogPostLikeRepository;
            this.userRepository = userRepository;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost]
        [Route("AddOrUpdateLike")]
        public async Task<IActionResult> AddOrUpdateLike([FromBody] AddLikeRequest addLikeRequest)
        {
            var existingLike = await blogPostLikeRepository.GetLikeByUserAndBlogPostAsync(addLikeRequest.UserId, addLikeRequest.BlogPostId);

            if (existingLike == null)
            {
                var model = new BlogPostLike
                {
                    BlogPostId = addLikeRequest.BlogPostId,
                    UserId = addLikeRequest.UserId,
                };

                await blogPostLikeRepository.AddLikeForBlog(model);
                _logger.LogInformation($"Added new like for blog post {addLikeRequest.BlogPostId} by user {addLikeRequest.UserId}");

                await SendLikeNotificationEmailAsync(addLikeRequest.UserId, addLikeRequest.BlogPostId);
            }
            else
            {
                _logger.LogInformation($"Like already exists for blog post {addLikeRequest.BlogPostId} by user {addLikeRequest.UserId}");
            }

            return Ok();
        }

        private async Task SendLikeNotificationEmailAsync(Guid userId, Guid blogPostId)
        {
            try
            {
                var emailConfig = _configuration.GetSection("EmailConfig");
                var fromEmail = emailConfig.GetValue<string>("FromEmail");
                var username = emailConfig.GetValue<string>("Username");
                var password = emailConfig.GetValue<string>("Password");
                var host = emailConfig.GetValue<string>("Host");
                var port = emailConfig.GetValue<int>("Port");

                var user = await GetUserByIdAsync(userId);
                var blogPost = await GetBlogPostByIdAsync(blogPostId);

                var message = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = "You've Liked a Blog Post on Bloogie!",
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
            <h1>Thank You for Your Like!</h1>
        </div>
        <div class='content'>
            <p>Dear {user.EmailAddress},</p>
            <p>Thank you for liking the blog post titled '{blogPost.Heading}' on Bloogie! We appreciate your engagement and hope you continue to enjoy our content.</p>
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

                message.To.Add(new MailAddress(user.EmailAddress));

                using (var mailClient = new SmtpClient(host, port))
                {
                    mailClient.UseDefaultCredentials = false;
                    mailClient.Credentials = new System.Net.NetworkCredential(username, password);
                    mailClient.EnableSsl = true;
                    mailClient.Timeout = 30000;
                    mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                    await mailClient.SendMailAsync(message);
                }

                _logger.LogInformation($"Like notification email sent to {user.EmailAddress} for blog post {blogPost.Heading}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending like notification email to user {userId}");
            }
        }

        private async Task<User> GetUserByIdAsync(Guid userId)
        {
            // Implement this method to get the user details from your user repository
            // For demonstration, returning a dummy user
            return await Task.FromResult(new User { EmailAddress = "user@example.com" });
        }

        private async Task<BlogPost> GetBlogPostByIdAsync(Guid blogPostId)
        {
            // Implement this method to get the blog post details from your blog post repository
            // For demonstration, returning a dummy blog post
            return await Task.FromResult(new BlogPost { Heading = "Sample Blog Post" });
        }

        public class User
        {
            public string EmailAddress { get; set; }
        }

        public class BlogPost
        {
            public string Heading { get; set; }
        }
    }
}
