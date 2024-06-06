using System.ComponentModel.DataAnnotations;
using Bloggie.Web.Models.Domain;

namespace Bloggie.Web.Models.ViewModel
{
    public class AddTagRequest
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string DisplayName { get; set; }
        
        public List<Tag> Tags { get; set; }
    }
}