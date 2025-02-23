using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TatliGezi.Models
{
    public class Article : BaseEntity
    {
        public string ArticleTitle { get; set; }

        public string ArticleContent { get; set; }
        public string MetaDescription { get; set; }

        public string QuoteContent { get; set; }
        public string QuoteAuthor { get; set; }

        public string PhotoPath1 { get; set; }
        public string ImageDescription1 { get; set; }

        public string PhotoPath2 { get; set; }
        public string ImageDescription2 { get; set; }
        public string PhotoPath3 { get; set; }
        public string ImageDescription3 { get; set; }
        public string Author { get; set; }
        public string Keywords { get; set; }

        public string Url { get; set; }
        public string Audio { get; set; }
        public bool IsActive { get; set; }

        public int View { get; set; }


        public virtual List<ArticleComment> ArticleComment { get; set; }
        public virtual List<ArticleTag> ArticleTag { get; set; }
      
        public Guid? ArticleCategoryID { get; set; }
        public virtual ArticleCategory ArticleCategory { get; set; }
        public Guid? FormatID { get; set; }
        public virtual Format Format { get; set; }


    }
}