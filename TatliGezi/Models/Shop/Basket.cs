using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TatliGezi.Models.Shop
{
    public class Basket :BaseEntity
    {
        public Guid? HostUserID { get; set; }
        public virtual HostUser HostUser { get; set; }
        public Guid ProductID { get; set; }
        public virtual Product Product { get; set; }
        public int Quantity { get; set; }
        public Guid? UserID { get; set; }
        public virtual User User { get; set; }

        public decimal ProductQuantityPrice { get; set; }
        public decimal ProductQuantityDiscountPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}