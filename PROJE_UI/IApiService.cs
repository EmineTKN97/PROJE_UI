using PROJE_UI.Models;

namespace PROJE_UI
{
    public interface IApiService
    {
        Task<List<Blog>> GetBlogsByCommentAndLikeCount();
        Task<Blog> GetBlogById(Guid blogId);
        Task<List<BlogComment>> GetCommentsByBlogId(Guid blogId);
        Task<BlogComment> AddComment(BlogComment comment);
    }
}

