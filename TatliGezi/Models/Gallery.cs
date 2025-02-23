using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TatliGezi.Models
{
    public class Gallery :BaseEntity
    {
        public string Url { get; set; }

        public string PhotoPath { get; set; }
        public string Content { get; set; }
        public bool IsSlider { get; set; }
    }
}