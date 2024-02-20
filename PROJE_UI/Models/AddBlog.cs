namespace PROJE_UI.Models
{
    public class AddBlog
    {
        public Guid BlogId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string? ImagePath { get; set; }
        public DateTime BlogDate { get; set; }
        public Guid UserId { get; set; }
    }
}
