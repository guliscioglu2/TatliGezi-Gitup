using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TatliGezi.Models
{
    public class BaseEntity
    {
        [Key]
        public Guid ID { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsDelete { get; set; }
    }
}