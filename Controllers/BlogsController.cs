using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
namespace Bloggie.Web.Controllers
{
    public class BlogsController : Controller
    {
        private readonly IBlogPostRepository blogPostRepository;
        private readonly ILogger<BlogsController> logger;

        public BlogsController(IBlogPostRepository blogPostRepository, ILogger<BlogsController> logger)
        {
            this.blogPostRepository = blogPostRepository;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string urlHandle)
        {
            var blogPost = await blogPostRepository.GetByUrlHandleAsync(urlHandle);
            return View(blogPost);
        }
    }
}
