using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TatliGezi.Models.Shop
{
    public class City
    {
        [Key]
        public Guid ID { get; set; }
        public string CityName { get; set; }
    }
}