namespace PROJE_UI.Models
{
    public class AdminPasswordChange
    {
        public string currentPassword { get; set; }
        public string newPassword { get; set; }
        public Guid AdminID { get; set; }
    }
}
