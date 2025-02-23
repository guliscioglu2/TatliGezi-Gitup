using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TatliGezi.Models
{
    public class ArticleCategory :BaseEntity
    {
        [Required(ErrorMessage = "Kategori adı boş geçilemez!!!")]
        public string CategoryName { get; set; }
        public bool IsActive { get; set; }
        public virtual List<ArticleCategoryJoin> ArticleCategoryJoin { get; set; }
    }
}