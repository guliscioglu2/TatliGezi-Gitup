using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TatliGezi.DTO;
using System.Web.Security;
using System.Security.Cryptography;
using TatliGezi.Models;
using TatliGezi.Models.Shop;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Web.Helpers;
using System.Text.RegularExpressions;
using System.Web.Routing;

namespace TatliGezi.Controllers
{
    public class LoginUserController : BaseController
    {
        // GET: LoginUser
        int pageSize = 7;

        [Route("girisyap")]
        public ActionResult Index(string searchString, int? page)
        {
            SomeCommonMethod();


            var arts = from m in db.Articles
                       select m;


            if (!String.IsNullOrEmpty(searchString))
            {
                return Redirect("/arama/" + searchString);



            }
            else
            {
                //if(ViewBag.returnUrl != null)
                //{
                    ViewBag.returnUrl = Request.Headers["Referer"].ToString();

                //}

                return View();

            }






        }

        [Route("girisyap")]
        [HttpPost]
        public ActionResult Index(LoginDTO user, string returnUrl)
        {
            SomeCommonMethod();

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
                var user1 = db.Users.Where(x => x.Email == user.Email && x.Password == user.Password && x.IsAdmin == false).FirstOrDefault();

                if (user1 != null /*&& user.RoleID == 1*/)
                {

                    FormsAuthentication.SetAuthCookie(user.Email, true);


                    HttpCookie myCookie = new HttpCookie("Member");
                    myCookie.Values["uID"] = Convert.ToString(user1.ID);
                    myCookie.Values["uName"] = Convert.ToString(user1.FullName);
                    myCookie.Expires = DateTime.Now.AddDays(1d);
                    Response.Cookies.Add(myCookie);


                    string ip1 = System.Web.HttpContext.Current.Request.UserHostAddress.ToString();
                    var hUser = db.HostUsers.Where(x => x.HostName == ip1).FirstOrDefault();

                    if (hUser != null)
                    {

                        List<Basket> pList = db.Baskets.Include("Product").Where(x => x.IsDelete == false && x.HostUserID == hUser.ID).OrderByDescending(x => x.AddDate).ToList();
                        List<Basket> puList = db.Baskets.Include("Product").Where(x => x.IsDelete == false && x.UserID == user1.ID).OrderByDescending(x => x.AddDate).ToList();
                     
                        HttpCookie myCookieH = new HttpCookie("HostUser");
                        myCookieH.Expires = DateTime.Now.AddDays(-1d);

                        foreach (var item in pList)
                        {
                            var product = db.Products.Where(x => x.ID == item.ProductID).FirstOrDefault();


                            if (puList.Count != 0)
                            {
                                var puList1 = db.Baskets.Include("Product").Where(x => x.IsDelete == false && x.UserID == user1.ID && x.ProductID == item.ProductID).FirstOrDefault();

                                if (puList1 != null)
                                {
                                    var totalQuantity = item.Quantity + puList1.Quantity;

                                    if (product.Stock >= totalQuantity)
                                    {
                                        puList1.Quantity += item.Quantity;
                                        puList1.ProductQuantityPrice = product.ProductPrice * totalQuantity;
                                        puList1.ProductQuantityDiscountPrice = product.ProductDiscountPrice * totalQuantity;
                                    }
                                    else
                                    {
                                        ViewBag.Alert = "Stokta Ürün bulunmamaktadır.";


                                    }
                                    item.IsDelete = true;

                                }

                                else
                                {
                          
                                    item.UserID = new Guid(Request.Cookies["Member"].Values["uID"]);
                                    item.HostUserID = null;

                                }
                                db.SaveChanges();

                            }
                            else
                            {
                                item.UserID = new Guid(Request.Cookies["Member"].Values["uID"]);
                                item.HostUserID = null;
                                db.SaveChanges();

                            }


                        }
                    }

                    if (!String.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    //fall back
                    return Redirect("/anasayfa/");

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

            HttpCookie myCookie = new HttpCookie("Member");
            myCookie.Expires = DateTime.Now.AddDays(-1d);
            Response.Cookies.Add(myCookie);
            return RedirectToAction("Index", "Home");

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
            user.ConfirmPassword = stbuilder.ToString();
            user.IsDelete = _user.IsDelete;
            db.Entry(_user).CurrentValues.SetValues(user);
            db.SaveChanges();

            return Json("");


        }

        [Route("uyeol")]
        public ActionResult Register(string searchString, int? page)
        {
            SomeCommonMethod();

            var arts = from m in db.Articles
                       select m;

            if (!String.IsNullOrEmpty(searchString))
            {

                return Redirect("/arama/" + searchString);



            }
            else
            {
                ViewBag.returnUrl = Request.Headers["Referer"].ToString();
                return View();

            }



        }

        [Route("uyeol")]
        [HttpPost]
        public ActionResult Register(User user, string returnUrl)
        {
            SomeCommonMethod();
            string _result = "";


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
                            user.IsAdmin = false;
                            user.Password = stbuilder.ToString();
                            user.ConfirmPassword = stbuilder.ToString();

                            db.Users.Add(user);
                            db.SaveChanges();

                            HttpCookie myCookie = new HttpCookie("Member");
                            myCookie.Values["uID"] = Convert.ToString(user.ID);
                            myCookie.Values["uName"] = Convert.ToString(user.FullName);
                            myCookie.Expires = DateTime.Now.AddDays(1d);
                            Response.Cookies.Add(myCookie);


                            string ip1 = System.Web.HttpContext.Current.Request.UserHostAddress.ToString();
                            var hUser = db.HostUsers.Where(x => x.HostName == ip1).FirstOrDefault();
                            if (hUser != null)
                            {

                                List<Basket> pList = db.Baskets.Include("Product").Where(x => x.IsDelete == false && x.HostUserID == hUser.ID).OrderByDescending(x => x.AddDate).ToList();
                                List<Basket> puList = db.Baskets.Include("Product").Where(x => x.IsDelete == false && x.UserID == user.ID).OrderByDescending(x => x.AddDate).ToList();

                                foreach (var item in pList)
                                {
                                    if (puList.Count != 0)
                                    {
                                        var puList1 = db.Baskets.Include("Product").Where(x => x.IsDelete == false && x.UserID == user.ID && x.ProductID == item.ProductID).FirstOrDefault();

                                        if (puList1 != null)
                                        {
                                            puList1.Quantity += item.Quantity;
                                            item.IsDelete = true;

                                        }

                                        else
                                        {
                                            item.UserID = new Guid(Request.Cookies["Member"].Values["uID"]);
                                            item.HostUserID = null;

                                        }
                                        db.SaveChanges();

                                    }
                                    else
                                    {
                                        item.UserID = new Guid(Request.Cookies["Member"].Values["uID"]);
                                        item.HostUserID = null;
                                        db.SaveChanges();

                                    }

                                }
                            }

                        }
                        catch (Exception err)
                        {
                            _result = err.ToString();
                        }

                        if (!String.IsNullOrEmpty(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        //fall back
                        return Redirect("/anasayfa/");
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



        [Route("profilim/{id}")]

        public ActionResult UserEdit(Guid id, string searchString, int? page)
        {
            SomeCommonMethod();



            if (!String.IsNullOrEmpty(searchString))
            {

                return Redirect("/arama/" + searchString);


            }
            else
            {

                User userF = db.Users.Find(id);

                return View(userF);




            }








        }

        [Route("profilim/{id}")]
        [HttpPost]
        public ActionResult UserEdit(User user)
        {
            ViewData["Kategori1"] = db.ArticleCategories.Where(x => x.IsDelete == false).ToList();

            if (user.Name != null && user.Surname != null && user.Email != null)
            {

                var _user = db.Users.Find(user.ID);
                user.AddDate = _user.AddDate;
                user.Password = _user.Password;
                user.UpdateDate = DateTime.Now;
                user.IsDelete = _user.IsDelete;
                user.IsAdmin = _user.IsAdmin;

                db.Entry(_user).CurrentValues.SetValues(user);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Durum = -1;
                return View();
            }
        }

        [Route("sifremiunuttum")]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [Route("sifremiunuttum")]
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
            var verifyUrl = "/sifreyenileme/" + emailFor + "/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("info@tatligezi.com", "Tatlı Gezi");
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
                message1.From = fromEmail;
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

        [Route("sifreyenileme/{emailFor}/{id}")]
        public ActionResult ResetPassword(string id)
        {
            //Verify the reset password link
            //Find account associated with this link
            //redirect to reset password page
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }

            {
                var user = db.Users.Where(a => a.ResetPasswordCode == id).FirstOrDefault();
                if (user != null)
                {
                    ResetPasswordModel model = new ResetPasswordModel();
                    model.ResetCode = id;
                    return View(model);
                }
                else
                {
                    return HttpNotFound();
                }
            }
        }

        [HttpPost]
        [Route("sifreyenileme/{emailFor}/{id}")]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                {
                    var user = db.Users.Where(a => a.ResetPasswordCode == model.ResetCode).FirstOrDefault();
                    if (user != null)
                    {
                        MD5 md5 = new MD5CryptoServiceProvider();
                        md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(model.NewPassword));
                        byte[] sonuc = md5.Hash;
                        StringBuilder stbuilder = new StringBuilder();
                        for (int i = 0; i < sonuc.Length; i++)
                        {
                            stbuilder.Append(sonuc[i].ToString("x2"));
                        }
                        user.Password = stbuilder.ToString();


                        //user.Password = System.Web.Helpers.Crypto.Hash(model.NewPassword);
                        user.ResetPasswordCode = "";
                        db.Configuration.ValidateOnSaveEnabled = false;
                        db.SaveChanges();
                        message = "Yeni şifreniz başarılı bir şekilde güncellenmiştir";
                    }
                }
            }
            else
            {
                message = "Bir sorun oluştu.";
            }
            ViewBag.Message = message;
            return View(model);
        }
    }
}