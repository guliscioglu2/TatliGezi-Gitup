using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TatliGezi.DTO
{
    public class PasswordDTO
    {
        [Required(ErrorMessage = "Parola boş geçilemez.")]
        public string ExPassword { get; set; }


        [Required(ErrorMessage = "Parola boş geçilemez.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Parola boş geçilemez.")]
        public string ComfirmNewPassword { get; set; }
    }
}