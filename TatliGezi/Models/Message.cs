using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TatliGezi.Models
{
    public class Message :BaseEntity
    {
        public string Name { get; set; }

        public string Mail { get; set; }

        public string Phone { get; set; }

        public string MessageContent { get; set; }

        public bool IsActive { get; set; }
    }
}