using System.ComponentModel.DataAnnotations;
using Bloggie.Web.Models.Domain;

namespace Bloggie.Web.Models.ViewModel
{
    public class AddTagRequest
    {
         [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

         [Required(ErrorMessage = "DisplayName is required")]
        public string DisplayName { get; set; }
        
        public List<Tag> Tags { get; set; }
    }
}