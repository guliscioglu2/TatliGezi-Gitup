using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TatliGezi.Models.Shop
{
    public class OrderProduct :BaseEntity
    {
        public Guid OrderID { get; set; }
        public Order Order { get; set; }
        public Guid ProductID { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public Guid? UserID { get; set; }
        public User User { get; set; }
        public decimal ProductQuantityPrice { get; set; }
        public decimal ProductQuantityDiscountPrice { get; set; }
    }
}