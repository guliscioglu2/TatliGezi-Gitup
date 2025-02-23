using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TatliGezi.DTO;
using TatliGezi.Models;

namespace TatliGezi.Areas.Admin.Controllers
{
    public class UserController : BaseAdminController
    {
        // GET: Admin/User
        public ActionResult Index()
        {
            if (Request.Cookies["Admin"] != null)
            {
                List<User> uList = db.Users.Where(x => x.IsDelete == false).ToList();

                return View(uList);
            }
            else
            {
                return Redirect("/Admin/LoginAdmin/Index/");
            }
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
            if (ModelState.IsValid)
            {

                var user1 = db.Users.Where(x => x.Email == user.Email).FirstOrDefault();
                if (user1 != null)
                {
                    ViewBag.Durum = 0;
                    return View();
                }
                MD5 md5 = new MD5CryptoServiceProvider();
                md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(user.Password));
                byte[] sonuc = md5.Hash;
                StringBuilder stbuilder = new StringBuilder();
                for (int i = 0; i < sonuc.Length; i++)
                {
                    stbuilder.Append(sonuc[i].ToString("x2"));
                }

                user.AddDate = DateTime.Now;
                user.UpdateDate = DateTime.Now;
                user.IsDelete = false;
                user.IsAdmin = true;
                user.Password = stbuilder.ToString();
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");


            }
            else
            {
                ViewBag.Durum = -1;
                return View();
            }
        }

        public ActionResult UserEdit(int id)
        {
            User user = db.Users.Find(id);

            if (Request.Cookies["Admin"] != null)
            {

                return View(user);
            }
            else
            {
                return Redirect("/Admin/LoginAdmin/Index/");
            }
        }
        [HttpPost]
        public ActionResult UserEdit(User user)
        {
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
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Durum = -1;
                return View();
            }
        }

        public JsonResult Delete(int id)
        {
            var del = db.Users.Find(id);
            del.IsDelete = true;
            db.SaveChanges();
            return Json("");
        }

        public ActionResult ChangePassword()
        {

            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(PasswordDTO user)
        {

            if (ModelState.IsValid)
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(user.ExPassword));
                byte[] sonuc = md5.Hash;
                StringBuilder stbuilder = new StringBuilder();
                for (int i = 0; i < sonuc.Length; i++)
                {
                    stbuilder.Append(sonuc[i].ToString("x2"));
                }
                user.ExPassword = stbuilder.ToString();

                Guid admin = new Guid(Request.Cookies["Admin"].Value);

                var user1 = db.Users.Where(x => x.Password == user.ExPassword && x.ID == admin).FirstOrDefault();

                if (user1 != null)
                {
                    if (user.NewPassword == user.ComfirmNewPassword)
                    {
                        MD5 md51 = new MD5CryptoServiceProvider();
                        md51.ComputeHash(ASCIIEncoding.ASCII.GetBytes(user.NewPassword));
                        byte[] sonuc1 = md51.Hash;
                        StringBuilder stbuilder1 = new StringBuilder();
                        for (int i = 0; i < sonuc1.Length; i++)
                        {
                            stbuilder1.Append(sonuc1[i].ToString("x2"));
                        }
                        user.NewPassword = stbuilder1.ToString();



                        user1.Password = user.NewPassword;
                        db.SaveChanges();
                    }
                    else
                    {
                        ViewBag.Durum = -1;
                        return View();
                    }
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
                        message = "New password updated successfully";
                    }
                }
            }
            else
            {
                message = "Something invalid";
            }
            ViewBag.Message = message;
            return View(model);
        }
    }
}