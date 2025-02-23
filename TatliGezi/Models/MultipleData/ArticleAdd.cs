using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TatliGezi.Models.MultipleData
{
    public class ArticleAdd
    {
        public Article article { get; set; }
        public List<Format> fList { get; set; }
        public ArticleTag tag { get; set; }
        public double value { get; set; }
    }
}