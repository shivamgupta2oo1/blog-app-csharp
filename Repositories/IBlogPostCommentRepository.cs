using Bloggie.Web.Models.Domain;

namespace Bloggie.Web.Repositories
{

    public interface IBlogPostCommentRepository
    {
        Task<BlogPostComment> AddAsync(BlogPostComment blogPostComment);
        Task<IEnumerable<BlogPostComment>> GetCommentByBlogIsAsync(Guid blogPostId);
    }

}
