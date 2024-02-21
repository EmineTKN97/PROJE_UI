namespace PROJE_UI.Models
{
    public class UserPaswordChange
    {
        public string currentPassword { get; set; }
        public string newPassword { get; set; }
        public Guid UserId { get; set; }
    }
}
