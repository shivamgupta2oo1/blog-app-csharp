using Bloggie.Web.Models.ViewModel;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bloggie.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminUserController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly UserManager<IdentityUser> userManager;

        public AdminUserController(IUserRepository userRepository, UserManager<IdentityUser> userManager)
        {
            this.userRepository = userRepository;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var users = await userRepository.GetAll();
            var userViewModel = new UserViewModel
            {
                Users = new List<User>()
            };
            foreach (var user in users)
            {
                userViewModel.Users.Add(new Models.ViewModel.User
                {
                    Id = Guid.Parse(user.Id),
                    UserName = user.UserName,
                    EmailAddress = user.Email
                });
            }
            return View(userViewModel);
        }


    }
}
