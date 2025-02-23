using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TatliGezi.Models
{
    public class ArticleTag :BaseEntity
    {
        public string TagName { get; set; }
        public virtual Article Article { get; set; }
        public Guid ArticleID { get; set; }
    }
}