using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TatliGezi.Models
{
    public class User :BaseEntity
    {
        [Required(ErrorMessage = "İsim boş geçilemez!")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Soyisim boş geçilemez!")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "Mail boş geçilemez!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre boş geçilez!")]
        [DisplayName("Password")]
        //[RegularExpression(@"^.*(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*\(\)_\-+=]).*$", ErrorMessage = "User_Password_Expression")]
        //[StringLength(45, MinimumLength = 8, ErrorMessage = "Minimum 2 & Maximum 45 character allowed")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public string PhoneNumber{ get; set; }

        public string PhotoPath { get; set; }

        public bool IsAdmin { get; set; }

        public string ResetPasswordCode { get; set; }

        public string FullName { get { return Name + " " + Surname; } }
    }
}