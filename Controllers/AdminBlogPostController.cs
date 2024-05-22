using Microsoft.AspNetCore.Mvc;

namespace Bloggie.Web.Controllers
{
    public class AdminBlogPostController : Controller
    {
        public IActionResult Add()
        {
            return View();
        }
    }
}
