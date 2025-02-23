using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TatliGezi.Models
{
    public class ForgetPassword: BaseEntity
    {
        public string Content { get; set; }
        public string Mail { get; set; }
        public Guid? UserID { get; set; }
        public virtual User User { get; set; }
    }
}