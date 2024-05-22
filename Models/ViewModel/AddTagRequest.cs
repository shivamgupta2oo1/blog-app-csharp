using Bloggie.Web.Models.Domain;

namespace Bloggie.Web.Models.ViewModel
{
    public class AddTagRequest
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public List<Tag> Tags { get; set; }
    }
}