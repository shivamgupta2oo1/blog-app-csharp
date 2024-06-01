using Bloggie.Web.Models.Domain;

namespace Bloggie.Web.Models.ViewModel
{
    public class BlogDetailsViewModel
    {
        public Guid Id { get; set; }
        public string Heading { get; set; }
        public string PageTitle { get; set; }
        public string Content { get; set; }
        public string ShortDescription { get; set; }
        public string FeatureImageUrl { get; set; }
        public string UrlHandle { get; set; }
        private DateTime _publishedDate;
        internal Guid id;

        public DateTime PublishedDate
        {
            get => _publishedDate;
            set => _publishedDate = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }
        public string Author { get; set; }
        public bool Visible { get; set; }
        public ICollection<Tag> Tags { get; set; }
        public int TotalLikes { get; set; }
        public bool Liked { get; set; }
    }
}