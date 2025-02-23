using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TatliGezi.Models
{
    public class Preview :BaseEntity
    {
        public string ArticleTitle { get; set; }

        public string ArticleContent { get; set; }

        public string PhotoPath1 { get; set; }
        public string PhotoPath2 { get; set; }

        public string PhotoPath3 { get; set; }
        public string QuoteContent { get; set; }

        public string Author { get; set; }
        public string Url { get; set; }
        public string Audio { get; set; }



        
        public Guid? ArticleCategoryID { get; set; }
        public virtual ArticleCategory ArticleCategory { get; set; }
        public int? FormatID { get; set; }
        public virtual Format Format { get; set; }


    }
}