namespace PROJE_UI.Models
{
    public class UserMedia
    {
        public Guid MediaId { get; set; }
        public Guid UserId { get; set; }
        public IFormFile  ImagePath { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
