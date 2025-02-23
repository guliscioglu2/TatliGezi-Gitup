using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TatliGezi.Models.Shop
{
    public class ProductComment :BaseEntity
    {

        public string CommentDetail { get; set; }

        public double Rate { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public Guid? ProductID { get; set; }

        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; }
        public Guid? UserID { get; set; }
        [ForeignKey("UserID")]

        public virtual User User { get; set; }
    }
}