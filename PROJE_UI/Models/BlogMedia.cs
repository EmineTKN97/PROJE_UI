namespace PROJE_UI.Models
{
    public class BlogMedia
    {
        public Guid MediaId { get; set; }
        public Guid BlogId { get; set; }
        public IFormFile ImagePath { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
