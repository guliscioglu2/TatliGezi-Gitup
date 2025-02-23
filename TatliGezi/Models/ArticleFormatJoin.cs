using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace TatliGezi.Models
{
    public class ArticleFormatJoin
    {
        public Guid ID { get; set; }
        public Guid ArticleID { get; set; }
        public virtual Article Article { get; set; }
        public Guid FormatID { get; set; }
        public virtual Format Format { get; set; }
    }
}