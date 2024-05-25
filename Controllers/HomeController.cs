using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Bloggie.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IBlogPostRepository blogPostRepository;

    public HomeController(ILogger<HomeController> logger, IBlogPostRepository blogPostRepository)
    {
        _logger = logger;
        this.blogPostRepository = blogPostRepository;
    }

    public async Task<IActionResult> Index()
    {
        var blogPosts = await blogPostRepository.GetAllAsync();
        return View(blogPosts);
    }
}
