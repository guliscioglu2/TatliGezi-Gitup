using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TatliGezi.Models.Shop
{
    public class Product :BaseEntity
    {
        public string ProductName { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Description { get; set; }
        public string MetaDescription { get; set; }

        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public int Stock { get; set; }
        public Guid? ProductCategoryID { get; set; }
        public ProductCategory ProductCategory { get; set; }
        public Guid? KdvRateID { get; set; }
        public KDVRate KdvRate { get; set; }
        public virtual List<ProductImage> ProductImages { get; set; }

        public virtual List<ProductComment> ProductComments { get; set; }
        public virtual List<ProductTag> ProductTag { get; set; }
        public string Keywords { get; set; }


        public string ImageView { get; set; }
        public double? AverageRate { get; set; }

        public decimal ProductPrice { get; set; }
        public decimal ProductDiscountPrice { get; set; }
    }
}