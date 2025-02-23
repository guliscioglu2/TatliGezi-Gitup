using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TatliGezi.Models.MultipleData;
using TatliGezi.Models;
using System.Web.Mail;
using System.Web.Helpers;
using System.Net.Mail;
using System.Net;
using TatliGezi.Models.Shop;
using ePayment;
using System.Globalization;
using TatliGezi.SeoSetting;
namespace TatliGezi.Controllers
{
    public class ShopController : BaseController
    {
        // GET: Shop
        [Route("magaza")]
        public ActionResult Index(string searchString, int? page)
        {
            SomeCommonMethod();

            MainPage _model = new MainPage();

            Session["Search"] = searchString;

            if (!String.IsNullOrEmpty(searchString))
            {

                return Redirect("/arama/" + searchString);


            }
            else
            {
                ViewData["ProductCategory"] = db.ProductCategories.Where(x => x.IsDelete == false).ToList();
                

                _model.pList = db.Products.Where(x => x.IsDelete == false).OrderByDescending(x => x.AddDate).ToList();
                _model.cList = db.ProductCategories.Where(x => x.IsDelete == false).OrderByDescending(x => x.AddDate).ToList();

                return View(_model);

            }

        }

        [ValidateInput(false)]
        [Route("urun/{title}")]
        public ActionResult Detail(string title, string searchString, int? page)
        {
            SomeCommonMethod();

            var pros = from m in db.Products
                       select m;
            foreach (var item in pros)
            {
                if (Seo.EditAdress(item.ProductName) == title)
                {
                    ViewData["ProductID"] = item.ID;
                }
            }

            Guid productID = Guid.Parse(ViewData["ProductID"].ToString());
            MainPage _model = new MainPage();
            Session["Search"] = searchString;


            if (!String.IsNullOrEmpty(searchString))
            {

                return Redirect("/arama/" + searchString);



            }
            else
            {
                _model.pList = db.Products.Where(x => x.IsDelete == false).OrderByDescending(x => x.AddDate).ToList();
                _model.product = db.Products.Where(x => x.ID == productID).FirstOrDefault();
                _model.kList = db.KDVRates.Where(x => x.IsDelete == false).OrderByDescending(x => x.AddDate).ToList();
                _model.cList = db.ProductCategories.Where(x => x.IsDelete == false).OrderByDescending(x => x.AddDate).ToList();
                var categoryID = db.Products.Where(x => x.IsDelete == false && x.ID == productID).First().ProductCategory.ID;
                ViewData["ProductList"] = db.Products.Where(x => x.ProductCategoryID == categoryID).Take(4).ToList();

                return View(_model);

            }
        }
     

        [ValidateInput(false)]
        [HttpPost]
        public JsonResult Comment(Guid id, string CommentDetail, double Rate, ProductComment comment)
        {
            var product = db.Products.Find(id);
            comment.ID = Guid.NewGuid();
            comment.CommentDetail = CommentDetail;
            var user1 = Request.Cookies["Member"];
            comment.UserID = Guid.Parse(user1.Values["uID"]);
            comment.ProductID = id;
            comment.Rate = Rate;
            comment.AddDate = DateTime.Now;
            comment.IsDelete = false;
            comment.UpdateDate = DateTime.Now;
            db.ProductComments.Add(comment);
            db.SaveChanges();

            var rateSum = db.ProductComments.Where(x=>x.ProductID ==id).Sum(x => x.Rate);
            var acount = db.ProductComments.Where(x => x.ProductID == id).Count();
            var average = rateSum / acount;
            product.AverageRate = average;
            db.SaveChanges();
            Guid z = id;
            return Json(z, JsonRequestBehavior.AllowGet);

        }

        public class UserCardInfo
        {
            public string CardNumber { get; set; }
            public string SecurityNumber { get; set; }

            public string CardHasName { get; set; }
            public int ExpMonth { get; set; }
            public int ExpYear { get; set; }

            public Guid ID { get; set; }

        }

        [Route("urunkategorisi/{title}")]
        public ActionResult ProductCategory(string title, string searchString, int? page)
        {
            SomeCommonMethod();

            var pros = from m in db.ProductCategories
                       select m;
            foreach (var item in pros)
            {
                if (Seo.EditAdress(item.CategoryName) == title)
                {
                    ViewData["CategoryID"] = item.ID;
                }
            }

            Guid categoryID = Guid.Parse(ViewData["CategoryID"].ToString());
            MainPage _model = new MainPage();

            Session["Search"] = searchString;
            if (!String.IsNullOrEmpty(searchString))
            {

                return Redirect("/arama/" + searchString);



            }
            else
            {
                ViewData["ProductCategory"] = db.ProductCategories.Where(x => x.IsDelete == false).ToList();
                _model.pList = db.Products.Where(x => x.IsDelete == false && x.ProductCategoryID == categoryID).OrderByDescending(x => x.AddDate).ToList();
                _model.cList = db.ProductCategories.Where(x => x.IsDelete == false).OrderByDescending(x => x.AddDate).ToList();
                _model.kList = db.KDVRates.Where(x => x.IsDelete == false).OrderByDescending(x => x.AddDate).ToList();

          
                return View(_model);

            }

        }
    }
}