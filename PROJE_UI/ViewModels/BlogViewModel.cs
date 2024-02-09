using PROJE_UI.Models;

namespace PROJE_UI.ViewModels
{
    public class BlogViewModel
    {
        public Blog Blogs { get; set; }
        public List<BlogComment> Comments { get; set; }

        public BlogComment NewComment { get; set; }
        public Guid BlogId { get; set; }

    }
}
