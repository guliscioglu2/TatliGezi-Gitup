using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TatliGezi.Models.Shop
{
    public class ProductImage :BaseEntity
    {
        public string PhotoPath { get; set; }
        public string ImageDescription { get; set; }
        public Guid? ProductID { get; set; }
        public virtual Product Product { get; set; }
    }
}