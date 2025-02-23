using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TatliGezi.Models
{
    public class Slider :BaseEntity
    {
        public string PhotoPath { get; set; }
 
        public string Content { get; set; }
        public string Url { get; set; }
    }
}