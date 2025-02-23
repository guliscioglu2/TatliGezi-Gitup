using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TatliGezi.Models.Shop;

namespace TatliGezi.Models.MultipleData
{
    public class MainPage
    {
        public Article article { get; set; }
        public Product product { get; set; }
        public Order order { get; set; }

        public List<Gallery> gList { get; set; }
        public List<Article> aList { get; set; }
        public List<Basket> bList { get; set; }

        public List<Product> pList { get; set; }
        public List<Order> oList { get; set; }
        public List<ProductCategory> cList { get; set; }
        public List<KDVRate> kList { get; set; }
        public ProductComment pComment { get; set; }



        public ArticleTag tag { get; set; }
        public double deger { get; set; }
    }
}