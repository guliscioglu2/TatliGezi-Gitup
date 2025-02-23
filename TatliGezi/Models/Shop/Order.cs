using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TatliGezi.Models.Shop
{
    public class Order :BaseEntity
    {
        public Guid UserID { get; set; }
        public User User { get; set; }
        public Guid UserAdressID { get; set; }
        public UserAdress UserAdress { get; set; }
        public Guid StatusID { get; set; }
        public Status Status { get; set; }
        public int OrderCode { get; set; }

        public decimal TotalPrice { get; set; }

        public virtual List<OrderPayment> OrderPayments { get; set; }
        public virtual List<OrderProduct> OrderProducts { get; set; }






    }
}
