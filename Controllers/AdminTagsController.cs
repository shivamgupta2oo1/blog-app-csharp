using System.Text.RegularExpressions;
using System.Xml.Linq;
using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModel;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Bloggie.Web.Controllers
{
    [Authorize(Roles = "Admin")]

    public class AdminTagsController : Controller
    {
        private readonly ITagRepository tagRepository;

        public AdminTagsController(ITagRepository tagRepository)
        {
            this.tagRepository = tagRepository;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Add")]
        public async Task<IActionResult> SubmitTag(AddTagRequest addTagRequest)
        {
            validateAddTagRequest(addTagRequest);
            if (ModelState.IsValid == false)
            {
                var tag = new Tag
                {
                    Name = addTagRequest.Name,
                    DisplayName = addTagRequest.DisplayName
                };

                await tagRepository.AddAsync(tag);
            }
            return RedirectToAction("List");
        }



        [HttpGet]
        public async Task<IActionResult> List(
            string? searchQuery,
            string? sortBy,
            string? sortDirection,
            int pageSize = 3,
            int pageNumber = 1
            )
        {

            var totalRecords = await tagRepository.CountAsync();
            var totalPages = Math.Ceiling((decimal)totalRecords / pageSize);

            if (pageNumber > totalPages)
            {
                pageNumber--;
            }

            if (pageNumber < 1)
            {
                pageNumber++;
            }

            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.PageNumber = pageNumber;
            ViewBag.SearchQuery = searchQuery;
            ViewBag.sortBy = sortBy;
            ViewBag.sortDirection = sortDirection;
            var tags = await tagRepository.GetAllAsync(searchQuery, sortBy, sortDirection, pageNumber, pageSize);
            // Console.WriteLine($"SearchData: {JsonConvert.SerializeObject(searchQuery)}");
            // Console.WriteLine($"sortBy: {JsonConvert.SerializeObject(sortBy)}");
            // Console.WriteLine($"sortDirection: {JsonConvert.SerializeObject(sortDirection)}");
            return View(tags);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var tag = await tagRepository.GetAsync(id);
            if (tag != null)
            {
                var editTagRequest = new EditTagRequest
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    DisplayName = tag.DisplayName,
                };
                return View(editTagRequest);
            }
            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditTagRequest editTagRequest)
        {
            var tag = new Tag
            {
                Id = editTagRequest.Id,
                Name = editTagRequest.Name,
                DisplayName = editTagRequest.DisplayName
            };

            var updatedTag = await tagRepository.UpdateAsync(tag);
            if (updatedTag != null)
            {
                //show success notification"
                await Task.Delay(2000);
                return RedirectToAction("List");
            }
            else
            {
                //show error notification
            }
            return RedirectToAction("Edit", new { id = editTagRequest.Id });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(EditTagRequest editTagRequest)
        {
            var deleteTag = await tagRepository.DeleteAsync(editTagRequest.Id);
            if (deleteTag != null)
            {
               await Task.Delay(3000);
            }
            else
            {
                //show error notification
            }
            return RedirectToAction("List", new { id = editTagRequest.Id });
        }
        public void validateAddTagRequest(AddTagRequest request)
        {
            if (request.Name != null && request.DisplayName != null)
            {
                if (request.DisplayName == request.Name)
                {
                    ModelState.AddModelError("DisplayName", " Name is not same as DisplayName");
                }
            }
        }
    }
}
