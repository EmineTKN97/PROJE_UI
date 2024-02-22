namespace PROJE_UI.Models
{
    public class BlogComment
    {
        public Guid CommentId { get; set; }
        public string CommentTitle { get; set; }
        public string UserName { get; set; }
        public string UserSurname { get; set; }
        public string CommentDetail { get; set; }
        public int? BlogLikeCount { get; set; }
        public DateTime CommentDate { get; set; }
        public string? UserİmagePath { get; set; }
        public Guid BlogId { get; set; }
        public string? BlogTitle { get; set; }
    }
}
