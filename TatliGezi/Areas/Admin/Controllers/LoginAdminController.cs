using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TatliGezi.DTO;
using TatliGezi.Models;

namespace TatliGezi.Areas.Admin.Controllers
{
    public class LoginAdminController : BaseAdminController
    {
        // GET: Admin/LoginAdmin

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(LoginDTO user)
        {
            if (ModelState.IsValid)
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(user.Password));
                byte[] sonuc = md5.Hash;
                StringBuilder stbuilder = new StringBuilder();
                for (int i = 0; i < sonuc.Length; i++)
                {
                    stbuilder.Append(sonuc[i].ToString("x2"));
                }
                user.Password = stbuilder.ToString();
                var user1 = db.Users.Where(x => x.Email == user.Email && x.Password == user.Password && x.IsAdmin == true).FirstOrDefault();
                if (user1 != null)
                {

                    FormsAuthentication.SetAuthCookie(user.Email, true);

                    HttpCookie myCookie = new HttpCookie("Admin");
                    myCookie.Values["uID"] = Convert.ToString(user1.ID);
                    myCookie.Values["uName"] = Convert.ToString(user1.FullName);
                    myCookie.Expires = DateTime.Now.AddDays(1d);
                    Response.Cookies.Add(myCookie);


                    return Redirect("/Admin/HomeAdmin/Index/");

                }
                else
                {
                    ViewBag.Durum = 0;
                    return View();
                }
            }
            else
            {
                ViewBag.Durum = -1;
                return View();
            }
        }

        public ActionResult SignOut()
        {
            HttpCookie myCookie = new HttpCookie("Admin");
            myCookie.Expires = DateTime.Now.AddDays(-1d);
            Response.Cookies.Add(myCookie);
            return RedirectToAction("Index");

        }

        public ActionResult UserAdd()
        {

            if (Request.Cookies["Admin"] != null)
            {


                return View();
            }
            else
            {
                return Redirect("/Admin/LoginAdmin/Index/");
            }

        }



        [HttpPost]
        public ActionResult UserAdd(User user)
        {
            string _result = "";


            var user2 = new User();
            if (ModelState.IsValid)
            {
                if (Regex.IsMatch(user.Password, @"^(?=.{8})(?=.*[a-z])(?=.*[A-Z])(?=.*\d)"))
                {

                    var user1 = db.Users.Where(x => x.Email == user.Email).FirstOrDefault();
                    if (user1 != null)
                    {
                        ViewBag.Durum = 0;
                        return View();
                    }
                    if (user.Password == user.ConfirmPassword)
                    {
                        try
                        {
                            MD5 md5 = new MD5CryptoServiceProvider();
                            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(user.Password));
                            byte[] sonuc = md5.Hash;
                            StringBuilder stbuilder = new StringBuilder();
                            for (int i = 0; i < sonuc.Length; i++)
                            {
                                stbuilder.Append(sonuc[i].ToString("x2"));
                            }
                            user.ID = Guid.NewGuid();
                            user.Name = user.Name;
                            user.Surname = user.Surname;
                            user.Email = user.Email;
                            user.AddDate = DateTime.Now;
                            user.UpdateDate = DateTime.Now;
                            user.IsDelete = false;
                            user.IsAdmin = true;
                            user.Password = stbuilder.ToString();
                            user.ConfirmPassword = stbuilder.ToString();

                            db.Users.Add(user);
                            db.SaveChanges();

                            HttpCookie myCookie = new HttpCookie("Admin");
                            myCookie.Values["uID"] = Convert.ToString(user.ID);
                            myCookie.Values["uName"] = Convert.ToString(user.FullName);
                            myCookie.Expires = DateTime.Now.AddDays(1);
                            Response.Cookies.Add(myCookie);

                        }
                        catch (Exception err)
                        {
                            _result = err.ToString();
                        }

                        return RedirectToAction("Index", "HomeAdmin");
                    }


                    else
                    {
                        ViewBag.Durum = -3;
                        return View();
                    }
                }
                else
                {
                    ViewBag.Durum = -2;
                    return View();
                }
            }
            else
            {
                ViewBag.Durum = -1;
                return View();
            }
        }




        public ActionResult ForgetPasswordList(ForgetPassword password)
        {

            List<ForgetPassword> fList = db.ForgetPasswords.Where(x => x.IsDelete == false).ToList();

            return View(fList);

        }
        public ActionResult UpdatePassword(Guid id)
        {
            ForgetPassword pass = db.ForgetPasswords.Find(id);

            string Mail = pass.Mail;
            var user = db.Users.Where(x => x.Email == Mail).FirstOrDefault();
            if (user != null)
            {
                pass.IsDelete = true;
                pass.UserID = user.ID;
                db.SaveChanges();
                return View(pass);
            }


            else
            {
                return RedirectToAction("ForgetPasswordList");
            }

        }

        public JsonResult UpdatePassword1(string Password, int ID, User user)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(Password));
            byte[] sonuc = md5.Hash;
            StringBuilder stbuilder = new StringBuilder();
            for (int i = 0; i < sonuc.Length; i++)
            {
                stbuilder.Append(sonuc[i].ToString("x2"));
            }


            var _user = db.Users.Find(ID);
            user.AddDate = _user.AddDate;
            user.Name = _user.Name;
            user.Surname = _user.Surname;
            user.Email = _user.Email;
            user.UpdateDate = DateTime.Now;
            user.Password = stbuilder.ToString();
            user.IsDelete = _user.IsDelete;
            db.Entry(_user).CurrentValues.SetValues(user);
            db.SaveChanges();

            return Json("");


        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string Email)
        {
            //Verify Email ID
            //Generate Reset password link 
            //Send Email 
            string message = "";
            bool status = false;

            //using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                var account = db.Users.Where(a => a.Email == Email).FirstOrDefault();
                if (account != null)
                {
                    //Send email for reset password
                    string resetCode = Guid.NewGuid().ToString();
                    SendVerificationLinkEmail(account.Email, resetCode, "ResetPassword");
                    account.ResetPasswordCode = resetCode;
                    //This line I have added here to avoid confirm password not match issue , as we had added a confirm password property 
                    //in our model class in part 1
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    message = "Şifre sıfırlama linki mail adresinize gönderildi.";
                }
                else
                {
                    message = "Hesap bulunamadı";
                }
            }
            ViewBag.Message = message;
            return View();
        }

        [NonAction]
        public void SendVerificationLinkEmail(string emailID, string activationCode, string emailFor = "VerifyAccount")
        {
            var verifyUrl = "/Admin/User/" + emailFor + "/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("info@tatligezi.com", "Dotnet Awesome");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "******"; // Replace with actual password

            string subject = "";
            string body = "";
            if (emailFor == "VerifyAccount")
            {
                subject = "Your account is successfully created!";
                body = "<br/><br/>We are excited to tell you that your Dotnet Awesome account is" +
                    " successfully created. Please click on the below link to verify your account" +
                    " <br/><br/><a href='" + link + "'>" + link + "</a> ";
            }
            else if (emailFor == "ResetPassword")
            {
                subject = "Reset Password";
                body = "Merhaba,<br/><br/>Hesap şifrenizi sıfırlamak için istek aldık. Şifrenizi sıfırlamak için lütfen aşağıdaki linke tıklayınız." +
                    "<br/><br/><a href=" + link + ">Reset Password link</a>";
            }

            SmtpClient smtpClient = new SmtpClient("mail.tatligezi.com", 587);
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new System.Net.NetworkCredential("info@tatligezi.com", "05359579170ZGul");
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.Port = 587;
            MailMessage message1 = new MailMessage();
            //message1.From = new MailAddress(fromEmail.ToString(), "Ekranda Görünecek İsim");


            message1.Subject = "Gizli yanıt";
            try
            {

                MailAddress fromAddress = new MailAddress(fromEmail.ToString(), toEmail.ToString());

                smtpClient.Host = "mail.tatligezi.com";
                message1.From = fromAddress;
                message1.To.Add(toEmail.ToString());
                message1.Subject = subject;
                message1.IsBodyHtml = true;
                message1.Body = body;
                //using (var message = new MailMessage(fromEmail, toEmail)
                //{
                //    Subject = subject,
                //    Body = body,
                //    IsBodyHtml = true
                //})

                smtpClient.Send(message1);

            }
            catch (Exception exp)
            {
                Response.Write(exp);

            }
            //SmtpClient sc = new SmtpClient();

            //sc.Port = 587;

            //sc.Host = "smtp.gmail.com";

            //sc.EnableSsl = true;

            //sc.Credentials = new NetworkCredential("guliscioglu89@gmail.com", "05359579170gul");

            //MailMessage mail = new MailMessage();

            //mail.From = new MailAddress("guliscioglu89@gmail.com", "Ekranda Görünecek İsim");


            //mail.Subject = "Gizli yanıt";

            //using (var message = new MailMessage(fromEmail, toEmail)
            //{
            //    Subject = subject,
            //    Body = body,
            //    IsBodyHtml = true
            //})
            //    sc.Send(message);
        }

    }
}