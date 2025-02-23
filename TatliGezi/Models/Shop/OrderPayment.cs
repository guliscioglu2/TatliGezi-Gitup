using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TatliGezi.Models.Shop
{
    public class OrderPayment :BaseEntity
    {
        public decimal Price { get; set; }

        public string Bank { get; set; }


    }

}
