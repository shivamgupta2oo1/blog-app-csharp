using Bloggie.Web.Models.Domain;

namespace Bloggie.Web.Repositories
{

    public interface IImageRepository
    {
        Task<string> UploadAsync(IFormFile file);
    }

}