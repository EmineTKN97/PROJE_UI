namespace PROJE_UI.Models
{
    public class AddBlog
    {
        public Guid İd { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string? İmagePath { get; set; }
        public DateTime BlogDate { get; set; }
        public Guid UserId { get; set; }
    }
}
