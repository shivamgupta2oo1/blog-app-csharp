using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModel;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> List()
        {
            var tags = await tagRepository.GetAllAsync();
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
                //show success notification
            }
            else
            {
                //show error notification
            }
            return RedirectToAction("Edit", new { id = editTagRequest.Id });
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
