
using Bloggie.Web.Models.Domain;

namespace Bloggie.Web.Repositories
{
    public interface IBlogPostLikeRepository
    {
        Task<int> GetTotalLikes(Guid blogPostId);
        Task<BlogPostLike> AddLikeForBlog(BlogPostLike blogPostLike);
        Task<IEnumerable<BlogPostLike>> GetLikeForBlog(Guid blogPostId);
         Task<BlogPostLike> GetLikeByUserAndBlogPostAsync(Guid userId, Guid blogPostId); 
    }
}