using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModel;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bloggie.Web.Controllers
{
    [Authorize(Roles = "Admin")]

    public class AdminBlogPostController : Controller
    {
        private readonly ITagRepository tagRepository;
        private readonly IBlogPostRepository blogPostRepository;

        public AdminBlogPostController(ITagRepository tagRepository, IBlogPostRepository blogPostRepository)
        {
            this.tagRepository = tagRepository;
            this.blogPostRepository = blogPostRepository;

        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var tags = await tagRepository.GetAllAsync();

            var model = new AddBlogPostRequest
            {
                Tags = tags.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddBlogPostRequest addBlogPostRequest)
        {

            //Map view model to domain model
            if (ModelState.IsValid == false)
            {
                var blogPost = new BlogPost
                {
                    Heading = addBlogPostRequest.Heading,
                    PageTitle = addBlogPostRequest.PageTitle,
                    Content = addBlogPostRequest.Content,
                    ShortDescription = addBlogPostRequest.ShortDescription,
                    FeatureImageUrl = addBlogPostRequest.FeatureImageUrl,
                    UrlHandle = addBlogPostRequest.UrlHandle,
                    PublishedDate = addBlogPostRequest.PublishedDate,
                    Author = addBlogPostRequest.Author,
                    Visible = addBlogPostRequest.Visible,
                };

                //Map Tags from Selected tags  
                var SelectedTags = new List<Tag>();
                foreach (var SelectedTagId in addBlogPostRequest.SelectedTags)
                {
                    var SelectedTagIdAsGuid = Guid.Parse(SelectedTagId);
                    var existingTag = await tagRepository.GetAsync(SelectedTagIdAsGuid);
                    if (existingTag != null)
                    {
                        SelectedTags.Add(existingTag);
                    }
                }
                blogPost.Tags = SelectedTags;
                await Task.Delay(2000);
                await blogPostRepository.AddAsync(blogPost);
            }
            return RedirectToAction("List");
        }

        public async Task<IActionResult> List(string? searchQuery, string? sortBy, string? sortDirection)
        {
            var blogPost = await blogPostRepository.GetAllAsync(searchQuery, sortBy, sortDirection);

            ViewBag.SearchQuery = searchQuery;
            ViewBag.sortBy = sortBy;
            ViewBag.sortDirection = sortDirection;
            return View(blogPost);
        }


        public async Task<IActionResult> Edit(Guid id)
        {
            var blogPost = await blogPostRepository.GetAsync(id);
            var tagDomainModel = await tagRepository.GetAllAsync();

            if (blogPost != null)
            {


                var model = new EditBlogPostRequest
                {
                    Id = blogPost.id,
                    Heading = blogPost.Heading,
                    PageTitle = blogPost.PageTitle,
                    Content = blogPost.Content,
                    ShortDescription = blogPost.ShortDescription,
                    FeatureImageUrl = blogPost.FeatureImageUrl,
                    UrlHandle = blogPost.UrlHandle,
                    PublishedDate = blogPost.PublishedDate,
                    Author = blogPost.Author,
                    Visible = blogPost.Visible,
                    Tags = tagDomainModel.Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString(),
                    }),
                    SelectedTags = blogPost.Tags.Select(x => x.Id.ToString()).ToArray()
                };
                return View(model);
            }
            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditBlogPostRequest editBlogPostRequest)
        {
            var blogPostDomainModel = new BlogPost
            {
                Id = editBlogPostRequest.Id,
                Heading = editBlogPostRequest.Heading,
                PageTitle = editBlogPostRequest.PageTitle,
                Content = editBlogPostRequest.Content,
                ShortDescription = editBlogPostRequest.ShortDescription,
                FeatureImageUrl = editBlogPostRequest.FeatureImageUrl,
                UrlHandle = editBlogPostRequest.UrlHandle,
                PublishedDate = editBlogPostRequest.PublishedDate,
                Author = editBlogPostRequest.Author,
                Visible = editBlogPostRequest.Visible,
            };
            var SelectedTags = new List<Tag>();
            foreach (var selectedTag in editBlogPostRequest.SelectedTags)
            {
                if (Guid.TryParse(selectedTag, out var tag))
                {
                    var foundTag = await tagRepository.GetAsync(tag);

                    if (foundTag != null)
                    {
                        SelectedTags.Add(foundTag);
                    }
                }
            }

            blogPostDomainModel.Tags = SelectedTags;

            var UpdateEdit = await blogPostRepository.UpdateAsync(blogPostDomainModel);

            if (UpdateEdit != null)
            {
                await Task.Delay(1000);
                return RedirectToAction("List");
            }
            return RedirectToAction("Edit");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(EditBlogPostRequest editBlogPostRequest)
        {
            var deleteBlogPost = await blogPostRepository.DeleteAsync(editBlogPostRequest.Id);
            if (deleteBlogPost != null)
            {
                await Task.Delay(1000);
                return RedirectToAction("List");
            }
            return RedirectToAction("Edit", new { id = editBlogPostRequest.Id });
        }
    }
}
