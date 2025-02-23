using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TatliGezi.Models.Shop
{
    public class Town
    {
        [Key]
        public Guid ID { get; set; }
        public string TownName { get; set; }
        public Guid? CityID { get; set; }
        public City City { get; set; }
    }
}