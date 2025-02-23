using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TatliGezi.Models
{
    public class ArticleCategoryJoin
    {
        public Guid ID { get; set; }
        public Guid ArticleID { get; set; }
        public virtual Article Article { get; set; }
        public Guid ArticleCategoryID { get; set; }
        public virtual ArticleCategory ArticleCategory { get; set; }
    }
}