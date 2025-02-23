using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TatliGezi.Models
{
    public class ArticleComment :BaseEntity
    {
        public string CommentDetail { get; set; }
        public Guid ArticleID { get; set; }
        public virtual Article Article { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }

        public Guid? UserID { get; set; }
        public virtual User User { get; set; }
    }
}