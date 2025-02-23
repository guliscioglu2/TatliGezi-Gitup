using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TatliGezi.Models.Shop
{
    public class UserAdress :BaseEntity
    {
        public string Title { get; set; }
        public string AdressDetail { get; set; }

        public Guid? UserID { get; set; }
        public User User { get; set; }
        public Guid? CityID { get; set; }
        public City City { get; set; }
        public Guid? TownID { get; set; }
        public Town Town { get; set; }
        public int PostCode { get; set; }
    }
}