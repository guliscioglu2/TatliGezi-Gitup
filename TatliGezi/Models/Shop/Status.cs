using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TatliGezi.Models.Shop
{
    public class Status :BaseEntity
    {
        public string Name { get; set; }
        public int Value { get; set; }

        public Guid? UserID { get; set; }
        public User User { get; set; }
    }
}