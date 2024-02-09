namespace PROJE_UI.Models
{
    public class Announcement
    {
        public Guid AnnouncementId { get; set; }
        public string AnnouncementTitle { get; set; }
        public string AnnouncementContent { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
