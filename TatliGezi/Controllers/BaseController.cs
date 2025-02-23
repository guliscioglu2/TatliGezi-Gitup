using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TatliGezi.Context;
using TatliGezi.Models;
using TatliGezi.Models.MultipleData;
using TatliGezi.Models.Shop;

namespace TatliGezi.Controllers
{
    public class BaseController : Controller
    {
        protected TatliGeziContext db;
        protected void SomeCommonMethod()
        {

            ViewData["LastList"] = db.Articles.Where(x => x.IsDelete == false && x.IsActive == true).OrderByDescending(x => x.AddDate).Take(4).ToList();
            ViewData["Category"] = db.ArticleCategories.Where(x => x.IsDelete == false).ToList();
            ViewData["Gallery"] = db.Galleries.Where(x => x.IsDelete == false).OrderByDescending(x => x.AddDate).Take(6).ToList();

            if (Request.Cookies["Member"] == null)

            {
                string ip1 = System.Web.HttpContext.Current.Request.UserHostAddress.ToString();

                var hUser = db.HostUsers.Where(x => x.HostName == ip1).FirstOrDefault();

                if (hUser == null)
                {
                    HostUser newhUser = new HostUser();
                    newhUser.ID = Guid.NewGuid();
                    newhUser.HostName = ip1;
                    db.HostUsers.Add(newhUser);
                    db.SaveChanges();
                    hUser = db.HostUsers.Where(x => x.HostName == ip1).FirstOrDefault();

                }



                HttpCookie myCookie = new HttpCookie("HostUser");
                myCookie.Values["uID"] = Convert.ToString(hUser.ID);
                myCookie.Expires = DateTime.Now.AddDays(1d);
                Response.Cookies.Add(myCookie);

                Session["Basket"] = db.Baskets.Where(x => x.IsDelete == false && x.HostUserID == hUser.ID).Count();
                ViewData["BasketList"] = db.Baskets.Include("Product").Where(x => x.IsDelete == false && x.HostUserID == hUser.ID).ToList();

                var basketListStandart = db.Baskets.Include("Product").Where(x => x.IsDelete == false && x.HostUserID == hUser.ID && x.Product.Discount == 0).ToList();
                var totalStandart = basketListStandart.Sum(x => x.ProductQuantityPrice);
                var basketListDiscount = db.Baskets.Include("Product").Where(x => x.IsDelete == false && x.HostUserID == hUser.ID && x.Product.Discount != 0).ToList();
                var totalDiscount = basketListDiscount.Sum(x => x.ProductQuantityDiscountPrice);
                var TotalPrice = totalStandart + totalDiscount;

                ViewData["Total"] = (totalStandart + totalDiscount).ToString("##.##");



            }

            else
            {
                Guid member = new Guid(Request.Cookies["Member"].Values["uID"]);

                Session["Basket"] = db.Baskets.Where(x => x.IsDelete == false && x.UserID == member).Count();
                ViewData["BasketList"] = db.Baskets.Include("Product").Where(x => x.IsDelete == false && x.UserID == member).ToList();

                var basketListStandart = db.Baskets.Include("Product").Where(x => x.IsDelete == false && x.UserID == member && x.Product.Discount == 0).ToList();
                var totalStandart = basketListStandart.Sum(x => x.ProductQuantityPrice);
                var basketListDiscount = db.Baskets.Include("Product").Where(x => x.IsDelete == false && x.UserID == member && x.Product.Discount != 0).ToList();
                var totalDiscount = basketListDiscount.Sum(x => x.ProductQuantityDiscountPrice);
                var TotalPrice = totalStandart + totalDiscount;

                ViewData["Total"] = (totalStandart + totalDiscount).ToString("##.##");

            }

        }
        protected void SomePaymentMethod()
        {

            Guid member = new Guid(Request.Cookies["Member"].Values["uID"]);

            List<Basket> bList = db.Baskets.Include("Product").Where(x => x.IsDelete == false && x.UserID == member).OrderByDescending(x => x.AddDate).ToList();

            var basketStandat = db.Baskets.Include("Product").Where(x => x.IsDelete == false && x.UserID == member && x.Product.Discount == 0).ToList();
            decimal totalStandart= basketStandat.Sum(x => x.ProductQuantityPrice); //indirimsiz ürünlerin fiyat toplamı

            var basketDiscount = db.Baskets.Include("Product").Where(x => x.IsDelete == false && x.UserID == member && x.Product.Discount != 0).ToList();
            decimal totalDiscount = basketDiscount.Sum(x => x.ProductQuantityDiscountPrice); //indirimli ürünlerin fiyat toplamı

            ViewData["TotalPrice"] = (totalStandart + totalDiscount).ToString("##.##");

            var cargo = db.Cargos.Where(x => x.IsDelete == false).FirstOrDefault();
            ViewData["CargoPrice"] = cargo.Price;

            ViewData["TotalPayment"] = ((totalStandart + totalDiscount) + cargo.Price).ToString("##.##");

            ViewData["ProductCategory"] = db.ProductCategories.Where(x => x.IsDelete == false).ToList();

            var city = db.Cities.ToList();
            var town = db.Towns.ToList();


        }
            public BaseController()
        {
            db = new TatliGeziContext();


        }

        [Route("arama/{searchString}")]
        public ActionResult Search(string searchString, string newSearchString)
        {
            SomeCommonMethod();
            MainPage _model = new MainPage();

            if (newSearchString != null)
            {
                //searchString = null;
                //var articles = db.Articles.Where(s => s.ArticleTitle.Contains(newSearchString));
                //var products = db.Products.Where(x => x.ProductName.Contains(newSearchString));
                //_model.aList = articles.ToList();
                //_model.pList = products.ToList();
               //return RedirectToAction("aramadetay/"+ new { newSearchString = newSearchString });
                return Redirect("/arama/" + newSearchString);

            }
            else
            {
                var arts = db.Articles.Where(s => s.ArticleTitle.Contains(searchString));
                var pros = db.Products.Where(x => x.ProductName.Contains(searchString));
                _model.aList = arts.ToList();
                _model.pList = pros.ToList();
                return View(_model);

            }


        }

    }
}