using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TatliGezi.Models
{
    public class Format :BaseEntity
    {
        [Required(ErrorMessage = "Kategori adı boş geçilemez!!!")]
        public string FormatName { get; set; }
        public int Value { get; set; }

        public virtual List<ArticleFormatJoin> ArticleFormatJoin { get; set; }
    }
}