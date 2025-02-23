using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TatliGezi.Models.Shop
{
    public class ConfirmedOrder :BaseEntity
    {
        public Guid ProductID { get; set; }

        public virtual Product Product { get; set; }
        public Guid UserID { get; set; }
        public User User { get; set; }
        public Guid UserAdressID { get; set; }
        public UserAdress UserAdress { get; set; }
        public int StatusID { get; set; }
        public Status Status { get; set; }
        public decimal TotalProductPrice { get; set; }
        public decimal TotalTaxPrice { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalPrice { get; set; }
        public virtual List<OrderPayment> OrderPayments { get; set; }
        public virtual List<OrderProduct> OrderProducts { get; set; }
        public int Quantity { get; set; }
    }
}