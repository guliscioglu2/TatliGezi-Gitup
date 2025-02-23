using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TatliGezi.Models.Shop
{
    public class ProductTag :BaseEntity
    {
        public string TagName { get; set; }
        public virtual Product Product { get; set; }
        public Guid ProductID { get; set; }
    }
}