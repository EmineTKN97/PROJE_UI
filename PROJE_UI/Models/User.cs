namespace PROJE_UI.Models
{
    public class User
    {
        public string Email { get; set; }
        public string? Password { get; set; }
        public string Name { get; set; }
        public string SurName { get; set; }
        public DateTime BirthDate { get; set; }
        public string? ImagePath { get; set; }
        public Guid? MediaId { get; set; }
        public Guid? UserId { get; set; }
    }
}
