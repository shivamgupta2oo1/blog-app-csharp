namespace Bloggie.Web.Models.Domain
{
    public class BlogPost
    {
        public Guid Id { get; set; }
        public string Heading { get; set; }
        public string PageTitle { get; set; }
        public string Content { get; set; }
        public string ShortDescription { get; set; }
        public string FeatureImageUrl { get; set; }
        public string UrlHandle { get; set; }
        public DateTime PUblishedDate { get; set; }
        public string Auther { get; set; }
        public bool Visible { get; set; }
        public  ICollection<Tag> Tags { get; set; }

    }
}