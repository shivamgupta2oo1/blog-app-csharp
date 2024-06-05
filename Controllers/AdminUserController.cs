// using Bloggie.Web.Models.ViewModel;
// using Bloggie.Web.Repositories;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Newtonsoft.Json;

// namespace Bloggie.Web.Controllers
// {
//     [Authorize(Roles = "Admin")]
//     public class AdminUserController : Controller
//     {
//         private readonly IUserRepository userRepository;
//         private readonly UserManager<IdentityUser> userManager;

//         public AdminUserController(IUserRepository userRepository, UserManager<IdentityUser> userManager)
//         {
//             this.userRepository = userRepository;
//             this.userManager = userManager;
//         }

//         [HttpGet]
//         public async Task<IActionResult> List()
//         {
//             var users = await userRepository.GetAll();
//             var userViewModel = new UserViewModel();
//             userViewModel.Users = new List<User>();
//             foreach (var user in users)
//             {
//                 userViewModel.Users.Add(new Models.ViewModel.User
//                 {
//                     Id = Guid.Parse(user.Id),
//                     UserName = user.UserName,
//                     EmailAddress = user.Email
//                 });
//             }
//             return View(userViewModel);
//         }

//         [HttpPost]
//         public async Task<IActionResult> List(UserViewModel request)
//         {
//             var identityUser = new IdentityUser
//             {
//                 UserName = request.Username,
//                 Email = request.Email
//             };
//             // Log the request object to the console
//             Console.WriteLine($"UserViewModel: {JsonConvert.SerializeObject(request)}");

//             // Return the view with the request model

//             var identityResult = await userManager.CreateAsync(identityUser, request.Password);
//             if (identityResult is not null)
//             {
//                 if (identityResult.Succeeded)
//                 {
//                     var roles = new List<string> { "User" };
//                     if (request.AdminRoleCheckBox)
//                     {
//                         roles.Add("Admin");
//                     }

//                     identityResult = await userManager.AddToRolesAsync(identityUser, roles);
//                     if (identityResult is not null && identityResult.Succeeded)
//                     {
//                         return RedirectToAction("List", "AdminUser");
//                     }
//                 }
//             }

//             return View();
//         }

//     }
// }



using Bloggie.Web.Models.ViewModel;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bloggie.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminUserController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly UserManager<IdentityUser> userManager;
        private readonly ILogger<AdminUserController> _logger;

        public AdminUserController(IUserRepository userRepository, UserManager<IdentityUser> userManager, ILogger<AdminUserController> logger)
        {
            this.userRepository = userRepository;
            this.userManager = userManager;
            _logger = logger;
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
                userViewModel.Users.Add(new User
                {
                    Id = Guid.Parse(user.Id),
                    UserName = user.UserName,
                    EmailAddress = user.Email
                });
            }
            return View(userViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> List(UserViewModel request)
        {
            Console.WriteLine($"UserViewModel: {JsonConvert.SerializeObject(request)}");

            if (ModelState.IsValid)
            {
                Console.WriteLine($"ModelState is valid");

                var identityUser = new IdentityUser
                {
                    UserName = request.Username,
                    Email = request.Email
                };
                Console.WriteLine($"Before CreateAsync: {JsonConvert.SerializeObject(identityUser)}");

                var identityResult = await userManager.CreateAsync(identityUser, request.Password);
                Console.WriteLine($"After CreateAsync: Succeeded={identityResult.Succeeded}");

                if (identityResult.Succeeded)
                {
                    var roles = new List<string> { "User" };
                    if (request.AdminRoleCheckBox)
                    {
                        roles.Add("Admin");
                    }

                    var roleResult = await userManager.AddToRolesAsync(identityUser, roles);
                    Console.WriteLine($"After AddToRolesAsync: Succeeded={roleResult.Succeeded}");

                    if (roleResult.Succeeded)
                    {
                        Console.WriteLine($"User and roles successfully created");
                        return RedirectToAction("List");
                    }

                    foreach (var error in roleResult.Errors)
                    {
                        Console.WriteLine($"Role error: {error.Description}");
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else
                {
                    foreach (var error in identityResult.Errors)
                    {
                        Console.WriteLine($"Identity error: {error.Description}");
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            else
            {
                Console.WriteLine($"ModelState is invalid");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Model error: {error.ErrorMessage}");
                }
            }

            var users = await userRepository.GetAll();
            Console.WriteLine($"Retrieved users: {JsonConvert.SerializeObject(users)}");

            request.Users = new List<User>();
            foreach (var user in users)
            {
                request.Users.Add(new User
                {
                    Id = Guid.Parse(user.Id),
                    UserName = user.UserName,
                    EmailAddress = user.Email
                });
            }

            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            if (user != null)
            {
                var identityResult = await userManager.DeleteAsync(user);
                if (identityResult != null && identityResult.Succeeded)
                {
                    return RedirectToAction("List", "AdminUser");
                }
            }
            return View();
        }

    }
}