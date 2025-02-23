using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TatliGezi.Models.Shop
{
    public class Sale :BaseEntity
    {
        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public string CardMonth { get; set; }
        public string CardYear { get; set; }
        public string CardCvv { get; set; }


    }
}