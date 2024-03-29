﻿namespace PROJE_UI.Models
{
    public class Blog
    {
        public Guid BlogId { get; set; }
        public string Title { get; set; }
        public DateTime BlogDate { get; set; }
        public string Content { get; set; }
        public int BlogLikeCount { get; set; }
        public int BlogCommentCount { get; set; }
        public string ImagePath { get; set; }
        public string Name { get; set; }
        public string SurName { get; set; }
        public string UserImagePath { get; set; }
        public Guid UserId { get; set; }
        public Guid? MediaId { get; set; }
        public Guid? LikeId { get; set; }


    }
}
