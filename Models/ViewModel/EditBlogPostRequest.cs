using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bloggie.Web.Models.ViewModel
{
    public class EditBlogPostRequest
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Heading is required.")]
        public string Heading { get; set; }

        [Required(ErrorMessage = "Page Title is required.")]
        public string PageTitle { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Short Description is required.")]
        public string ShortDescription { get; set; }

        [Required(ErrorMessage = "Feature Image URL is required.")]
        public string FeatureImageUrl { get; set; }

        [Required(ErrorMessage = "URL Handle is required.")]
        public string UrlHandle { get; set; }

        [Required(ErrorMessage = "Published Date is required.")]
        public DateTime PublishedDate { get; set; }

        [Required(ErrorMessage = "Author is required.")]
        public string Author { get; set; }

        public bool Visible { get; set; }

        public IEnumerable<SelectListItem> Tags { get; set; }

        [Required(ErrorMessage = "At least one tag must be selected.")]
        public string[] SelectedTags { get; set; } = Array.Empty<string>();
    }
}