using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TatliGezi.DTO
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Mail boş geçilemez.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Parola boş geçilemez.")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}