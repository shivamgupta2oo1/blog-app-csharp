using Bloggie.Web.Models.Domain;

namespace Bloggie.Web.Models.ViewModel
{
    public class HomeViewModel
    {
        public IEnumerable<BlogPost> BlogPosts { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
        private DateTime _publishedDate;
        internal Guid id;

        public DateTime PublishedDate
        {
            get => _publishedDate;
            set => _publishedDate = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }
    }

}