using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TatliGezi.Models
{
    public class ArticleTagJoin
    {
        public Guid ID { get; set; }
        public Guid ArticleID { get; set; }
        public virtual Article Article { get; set; }
        public Guid ArticleTagID { get; set; }
        public virtual ArticleTag ArticleTag { get; set; }
    }
}