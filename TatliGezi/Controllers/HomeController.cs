using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using TatliGezi.Models;
using TatliGezi.Models.MultipleData;
using TatliGezi.SeoSetting;
using System.Web.UI.WebControls;

namespace TatliGezi.Controllers
{
    public class HomeController : BaseController
    {
        int pageSize = 7;


        public void auth(string title)
        {

        }

        [Route("")]
        [Route("Home/Index")]
        [Route("anasayfa")]
        [Route("[controller]/[action]")]
        public ActionResult Index(string searchString, int? page)
        {

            SomeCommonMethod();


            if (!String.IsNullOrEmpty(searchString))
            {

                return Redirect("/arama/" + searchString);



            }

            else
            {
                ViewData["SliderGallery"] = db.Sliders.Where(x => x.IsDelete == false).OrderByDescending(x => x.AddDate).Take(20).ToList();

                if (!page.HasValue)
                {
                    List<Article> aList = db.Articles.Where(x => x.IsDelete == false && x.IsActive == true).OrderByDescending(x => x.AddDate).Take(pageSize).ToList();

                    return View(aList);

                }
                else
                {
                    int pageIndex = pageSize * page.Value;
                    List<Article> aList = db.Articles.Where(x => x.IsDelete == false && x.IsActive == true).OrderByDescending(x => x.AddDate).Skip(pageIndex).Take(pageSize).ToList();


                    if (Request.IsAjaxRequest())
                    {
                        return PartialView("Partial/Article", aList);
                    }

                    return View(aList);


                }


            }

        }


        [Route("blog/{title}")]
        public ActionResult Detail(string title, string searchString)
        {
            SomeCommonMethod();

            var arts = from m in db.Articles
                       select m;

            Session["Search"] = searchString;

            if (!String.IsNullOrEmpty(searchString))
            {

                return Redirect("/Arama/" + searchString);


            }

            else
            {
                //var supplier = (from m in dataContext.BO_Suppliers
                //                where m.SupplierID == id
                //                select m).FirstOrDefault();

                //ViewData.Model = supplier;
                //var art = (from c in db.Articles
                //           where (Seo.EditAdress(c.ArticleTitle)) == title

                //           orderby c.ArticleTitle descending
                //           select c)
                //.FirstOrDefault();


                //var edit = db.Articles.Find();
                //edit.ArticleTitle = Seo.EditAdress(edit.ArticleTitle);

                //var chwWorker = (from c in db.Articles
                //                 where c.ArticleTitle == title).FirstOrDefault();

                foreach (var item in arts)
                {
                    if (Seo.EditAdress(item.ArticleTitle) == title)
                    {
                        ViewData["BlogID"] = item.ID;
                    }
                }
                //var articleId = db.Articles.FirstOrDefault(x => Seo.EditAdress(x.ArticleTitle) == title);

                Guid blogID = Guid.Parse(ViewData["BlogID"].ToString());
                var article = db.Articles.Find(blogID);
                article.View += 1;
                db.SaveChanges();

                Session["ArticleID"] = blogID;

                ViewData["EnBuyuk"] = db.Articles.Where(x => x.IsDelete == false).Max(x => x.AddDate);
                ViewData["EnKucuk"] = db.Articles.Where(x => x.IsDelete == false).Min(x => x.AddDate);
                ViewData["Tags"] = db.ArticleTags.Where(x => x.IsDelete == false && x.ArticleID == blogID).ToList();
                ViewData["Posts"] = db.Articles.Where(x => x.IsDelete == false && x.ArticleCategoryID == article.ArticleCategoryID).OrderByDescending(x => x.AddDate).Take(4).ToList();


                List<Article> artList = db.Articles.Where(x => x.IsDelete == false).OrderByDescending(x => x.AddDate).ToList();

                List<Guid> aList = new List<Guid>();

                for (var i = 0; i < artList.Count(); i++)
                {
                    aList.Add(artList[i].ID);
                }


                for (var j = 0; j < aList.Count(); j++)
                {
                    if (blogID == aList[j])
                    {
                        if (article.AddDate != Convert.ToDateTime(ViewData["EnKucuk"]))
                        {
                            Guid m = aList[j + 1];

                            Article art1 = db.Articles.Find(m);
                            ViewData["NextName"] = art1.ArticleTitle;
                        }


                        if (article.AddDate != Convert.ToDateTime(ViewData["EnBuyuk"]))
                        {
                            Guid n = aList[j - 1];

                            Article art2 = db.Articles.Find(n);
                            ViewData["PreName"] = art2.ArticleTitle;
                        }
                    }

                }


                return View(article);



            }


        }
        public ActionResult TagList(Guid id, string searchString, int? page)
        {
            SomeCommonMethod();


            var arts = from m in db.Articles
                       select m;

            MainPage _model = new MainPage();
            Session["Search"] = searchString;

            if (!String.IsNullOrEmpty(searchString))
            {

                arts = db.Articles.Where(s => s.ArticleTitle.Contains(searchString));

                List<Article> aList = arts.ToList();

                return View(aList);


            }
            else
            {


                List<Guid> TagList = new List<Guid>();
                ArticleTag tag = db.ArticleTags.Find(id);
                List<ArticleTag> tagList = db.ArticleTags.Where(x => x.IsDelete == false).ToList();
                foreach (var item in tagList)
                {
                    if (item.TagName == tag.TagName)
                    {
                        TagList.Add(item.ArticleID);
                    }
                }

                List<Article> artList = new List<Article>();

                List<Article> articleList = db.Articles.Where(x => x.IsDelete == false).ToList();

                foreach (var item in articleList)
                {
                    foreach (var item1 in TagList)
                    {
                        if (item.ID == item1)
                        {
                            artList.Add(item);
                        }

                    }
                }

                _model.aList = artList.ToList();




                return View(_model);



            }

        }

        [Route("kategori/{title}")]
        public ActionResult CategoryPage(string title, string searchString, int? page)
        {
            SomeCommonMethod();

            var arts = from m in db.Articles
                       select m;

            var cats = from m in db.ArticleCategories
                       select m;

            foreach (var item in cats)
            {
                if (Seo.EditAdress(item.CategoryName) == title)
                {
                    ViewData["CategoryID"] = item.ID;
                }
            }

            Guid catID = Guid.Parse(ViewData["CategoryID"].ToString());

            var category = db.ArticleCategories.Find(catID);

            ViewData["CategoryName"] = category.CategoryName;


            if (!String.IsNullOrEmpty(searchString))
            {

                return Redirect("/arama/" + searchString);



            }
            else
            {
                if (!page.HasValue)
                {
                    List<Article> aList = db.Articles.Where(x => x.IsDelete == false && x.IsActive == true && x.ArticleCategoryID == category.ID).OrderByDescending(x => x.AddDate).Take(pageSize).ToList();


                    return View(aList);

                }
                else
                {
                    int pageIndex = pageSize * page.Value;
                    List<Article> aList = db.Articles.Where(x => x.IsDelete == false && x.IsActive == true && x.ArticleCategoryID == category.ID).OrderByDescending(x => x.AddDate).Skip(pageIndex).Take(pageSize).ToList();


                    if (Request.IsAjaxRequest())
                    {
                        return PartialView("Partial/Article", aList);
                    }

                    return View(aList);

                }



            }

        }

        [Route("galeri")]
        public ActionResult Gallery(string searchString, int? page)
        {

            SomeCommonMethod();


            var arts = from m in db.Articles
                       select m;

            ViewData["Search"] = searchString;

            MainPage _model = new MainPage();

            if (!String.IsNullOrEmpty(searchString))
            {

                arts = db.Articles.Where(s => s.ArticleTitle.Contains(searchString));

                List<Article> aList = arts.ToList();

                return View(aList);

            }
            else
            {

                _model.gList = db.Galleries.Where(x => x.IsDelete == false).OrderByDescending(x => x.AddDate).ToList();

                return View(_model);

            }

        }

        [HttpPost]
        public ActionResult Comment1(string CommentDetail, string Name, string Email, ArticleComment comment, Article article)
        {
            var articleID = Session["ArticleID"].ToString();
            comment.ID = Guid.NewGuid();
            comment.CommentDetail = CommentDetail;
            comment.Name = Name;
            comment.Email = Email;

            comment.ArticleID = Guid.Parse(articleID);
            comment.AddDate = DateTime.Now;
            comment.IsDelete = false;
            comment.UpdateDate = DateTime.Now;
            db.ArticleComments.Add(comment);
            db.SaveChanges();
            Guid z = comment.ArticleID;
            return Json(z, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Comment2(string CommentDetail, ArticleComment comment, Article article)
        {
            var articleID = Session["ArticleID"].ToString();
            comment.ID = Guid.NewGuid();
            comment.CommentDetail = CommentDetail;
            Guid user1 = new Guid(Request.Cookies["Member"].Values["uID"]);
            comment.UserID = user1;
            comment.ArticleID = Guid.Parse(articleID);
            comment.AddDate = DateTime.Now;
            comment.IsDelete = false;
            comment.UpdateDate = DateTime.Now;
            db.ArticleComments.Add(comment);
            db.SaveChanges();
            Guid z = comment.ArticleID;
            return Json(z, JsonRequestBehavior.AllowGet);

        }


        public ActionResult PrevPage(Guid id)
        {
            Article article = db.Articles.Find(id);
            List<Article> artList = db.Articles.Where(x => x.IsDelete == false && x.Format.Value != 2 && x.Format.Value != 6).OrderByDescending(x => x.AddDate).ToList();

            List<Guid> aList = new List<Guid>();

            for (var i = 0; i < artList.Count(); i++)
            {
                aList.Add(artList[i].ID);
            }


            for (var j = 0; j < aList.Count(); j++)
            {
                if (id == aList[j])
                {
                    Guid m = aList[j - 1];
                    Article art = db.Articles.Find(m);
                    Session["PrevPageID"] = m;
                }

            }

            return RedirectToAction("Detay", new { id = Session["PrevPageID"] });



        }


        public ActionResult NextPage(Guid id)
        {
            ViewBag.deneme = "sdasdas";

            Article article = db.Articles.Find(id);

            List<Article> artList1 = db.Articles.Where(x => x.IsDelete == false && x.Format.Value != 2 && x.Format.Value != 6).OrderByDescending(x => x.AddDate).ToList();

            List<Guid> aList1 = new List<Guid>();

            for (var i = 0; i < artList1.Count(); i++)
            {
                aList1.Add(artList1[i].ID);
            }


            for (var j = 0; j < aList1.Count(); j++)
            {
                if (id == aList1[j])
                {
                    Guid m = aList1[j + 1];
                    Article art = db.Articles.Find(m);
                    Session["NextPageID"] = m;
                }

            }


            return RedirectToAction("Detay", new { id = Session["NextPageID"] });



        }

        [Route("hakkimizda")]
        public ActionResult About(string searchString, int? page)
        {

            SomeCommonMethod();


            var arts = from m in db.Articles
                       select m;


            MainPage _model = new MainPage();
            Session["Search"] = searchString;


            if (!String.IsNullOrEmpty(searchString))
            {

                return Redirect("/arama/" + searchString);



            }
            else
            {

                List<Article> aList = db.Articles.Where(x => x.IsDelete == false && x.IsActive == true).OrderByDescending(x => x.AddDate).Take(pageSize).ToList();

                return View();





            }

        }
        [Route("iletisim")]

        public ActionResult Contact(string searchString, int? page)
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

                return View();






            }



        }

        [Route("iletisim")]
        [HttpPost]
        public ActionResult Contact(Message message, string Name, string Mail, string MessageContent)
        {
            SomeCommonMethod();


            var arts = from m in db.Articles
                       select m;
            message.ID = Guid.NewGuid();
            message.AddDate = DateTime.Now;
            message.IsDelete = false;
            message.UpdateDate = DateTime.Now;
            db.Messages.Add(message);
            db.SaveChanges();

            Name = message.Name;
            Mail = message.Mail;
            MessageContent = message.MessageContent;

            

            var fromEmail = new MailAddress(Mail);
            var toEmail = new MailAddress("info@tatligezi.com");
            string subject = "";
            string body = "";

            subject = "Your account is successfully created!";
            body = "<br/><br/>We are excited to tell you that your Dotnet Awesome account is" +
                Name +
                Mail +
                MessageContent +
                " successfully created. Please click on the below link to verify your account" +
                " <br/><br/> ";


            SmtpClient smtpClient = new SmtpClient("mail.tatligezi.com", 587);
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new System.Net.NetworkCredential("info@tatligezi.com", "05359579170ZGul");
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.Port = 587;
            MailMessage message1 = new MailMessage();

            try
            {
                //775
                //MailAddress fromAddress = new MailAddress("info@tatligezi.com", fromEmail.ToString() );

                //smtpClient.Host = "mail.tatligezi.com";
                //message1.From = fromAddress;
                //message1.To.Add("info@tatligezi.com");
                //message1.Subject = subject;
                //message1.IsBodyHtml = true;
                //message1.Body = body;
                //smtpClient.Send(message1);
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
                   
                    //ServicePointManager.ServerCertificateValidationCallback = delegate (object s, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                    //    System.Security.Cryptography.X509Certificates.X509Chain chain,
                    //    System.Net.Security.SslPolicyErrors sslPolicyErrors) { return true; };

            var result = new { Success = "true", Message = "Mesajını gönderildi" };
            TempData["ProcessMessage"] = "Email Sent Successfully.";
            return Json(new { result }, JsonRequestBehavior.AllowGet);

        }

        [Route("mesafeli-satis-sozlesmesi")]
        public ActionResult DistanceSaleContract(string searchString, int? page)
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

                return View();


            }



        }
        [Route("teslimat-ve-iade")]

        public ActionResult DeliveryAndReturns(string searchString, int? page)
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

                return View();


            }



        }
        [Route("gizlilik-politikasi")]
        public ActionResult PrivacyPolicy(string searchString, int? page)
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

                return View();


            }



        }

        [Route("cerez-politikasi")]
        public ActionResult CookiePolicy(string searchString, int? page)
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

                return View();


            }



        }

        [Route("yakinda")]
        public ActionResult ComingSoon(string searchString, int? page)
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

                return View();


            }



        }
    }
}