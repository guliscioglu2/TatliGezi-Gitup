using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TatliGezi.Models.Shop
{
    public class ProductCategory :BaseEntity
    {
        [Required(ErrorMessage = "Kategori adı boş geçilemez!!!")]
        public string CategoryName { get; set; }
        public bool IsActive { get; set; }
    }
}