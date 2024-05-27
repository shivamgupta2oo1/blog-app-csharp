using Bloggie.Web.Models.ViewModel;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Bloggie.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> logger;
    private readonly IBlogPostRepository blogPostRepository;
    private readonly ITagRepository tagRepository;

    public HomeController(ILogger<HomeController> logger, IBlogPostRepository blogPostRepository, ITagRepository tagRepository)
    {
        this.logger = logger;
        this.blogPostRepository = blogPostRepository;
        this.tagRepository = tagRepository;
    }

    public async Task<IActionResult> Index()
    {
        var blogPosts = await blogPostRepository.GetAllAsync();
        var tags = await tagRepository.GetAllAsync();
        var model = new HomeViewModel
        {
            BlogPosts = blogPosts,
            Tags = tags,
        };
        return View(model);
    }
}
