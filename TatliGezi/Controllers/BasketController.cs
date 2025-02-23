using TatliGezi.DTO;
using TatliGezi.Models;
using TatliGezi.Models.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace TatliGezi.Controllers
{
    public class BasketController : BaseController
    {
        // GET: Basket
        int pageSize = 7;

        // GET: Basket
        [HttpPost]
        public JsonResult UpQuantity(Guid id)
        {

            var product = db.Products.Find(id);

            //var product = db.Products.Where(x => x.ID == quantity.Product.ID).FirstOrDefault();

            if (Request.Cookies["Member"] == null)
            {


                var hUser = new Guid(Request.Cookies["HostUser"].Values["uID"]);
                var pBasket = db.Baskets.Where(x => x.HostUserID == hUser && x.IsDelete == false && x.ProductID ==product.ID && x.UserID == null).FirstOrDefault();

                if (pBasket == null)
                {
                    var totalQuantity = pBasket.Quantity += 1;

                    if (product.Stock >= totalQuantity)
                    {
                        pBasket.Quantity = totalQuantity;
                        pBasket.ProductQuantityPrice = product.ProductPrice * totalQuantity;
                        pBasket.ProductQuantityDiscountPrice = product.ProductDiscountPrice * totalQuantity;

                    }

                    else
                    {
                        ViewBag.Alert = "Stokta Ürün bulunmamaktadır.";


                    }
                }

              
            }
            else
            {
                Guid member = new Guid(Request.Cookies["Member"].Values["uID"]);

                var pBasket = db.Baskets.Where(x => x.HostUserID == null && x.IsDelete == false && x.ProductID == product.ID && x.UserID == member).FirstOrDefault();
            
                    var totalQuantity = pBasket.Quantity += 1;

                    if (product.Stock >= totalQuantity)
                    {
     

                        pBasket.Quantity = totalQuantity;
                        pBasket.ProductQuantityPrice = product.ProductPrice * totalQuantity;
                        pBasket.ProductQuantityDiscountPrice = product.ProductDiscountPrice * totalQuantity;
                  
                }

                    else
                    {
                        ViewBag.Alert = "Stokta Ürün bulunmamaktadır.";

                }
                }
           
            var rt = db.SaveChanges();
            return Json(new { rt, ViewBag.Alert }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult DownQuantity(Guid id)
        {

            var product = db.Products.Find(id);

            if (Request.Cookies["Member"] == null)
            {


                var hUser = new Guid(Request.Cookies["HostUser"].Values["uID"]);
                var pBasket = db.Baskets.Where(x => x.HostUserID == hUser && x.IsDelete == false && x.ProductID == product.ID && x.UserID == null).FirstOrDefault();

                if (pBasket == null)
                {
                    var totalQuantity = pBasket.Quantity -= 1;

                    if (product.Stock >= totalQuantity)
                    {
                        pBasket.Quantity = totalQuantity;
                        pBasket.ProductQuantityPrice = product.ProductPrice * totalQuantity;
                        pBasket.ProductQuantityDiscountPrice = product.ProductDiscountPrice * totalQuantity;

                    }

                    else
                    {
                        ViewBag.Alert = "Stokta Ürün bulunmamaktadır.";


                    }
                }


            }
            else
            {
                Guid member = new Guid(Request.Cookies["Member"].Values["uID"]);

                var pBasket = db.Baskets.Where(x => x.HostUserID == null && x.IsDelete == false && x.ProductID == product.ID && x.UserID == member).FirstOrDefault();

                var totalQuantity = pBasket.Quantity -= 1;

                if (product.Stock >= totalQuantity)
                {


                    pBasket.Quantity = totalQuantity;
                    pBasket.ProductQuantityPrice = product.ProductPrice * totalQuantity;
                    pBasket.ProductQuantityDiscountPrice = product.ProductDiscountPrice * totalQuantity;

                }

                else
                {
                    ViewBag.Alert = "Stokta Ürün bulunmamaktadır.";

                }
            }

            var rt = db.SaveChanges();
            return Json(new { rt, ViewBag.Alert }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult CleanBasket()
        {

            if (Request.Cookies["Member"] == null)
            {
                string ip1 = System.Web.HttpContext.Current.Request.UserHostAddress.ToString();
                var hUser = db.HostUsers.Where(x => x.HostName == ip1).FirstOrDefault();
                if (hUser != null)
                {
                    Guid h = hUser.ID;
                    List<Basket> bList = db.Baskets.Include("Product").Where(x => x.IsDelete == false && x.HostUserID == h).ToList();
                    foreach (var item in bList)
                    {
                        item.IsDelete = true;
                    }
                }
                else
                {
                    HostUser user = new HostUser();
                    user.HostName = ip1;
                    db.HostUsers.Add(user);
                    db.SaveChanges();
                    Guid h = user.ID;
                    List<Basket> bList = db.Baskets.Include("Product").Where(x => x.IsDelete == false && x.HostUserID == user.ID).ToList();
                    foreach (var item in bList)
                    {
                        item.IsDelete = true;
                    }
                }

            }
            else
            {
                Guid member = new Guid(Request.Cookies["Member"].Values["uID"]);

                List<Basket> bList = db.Baskets.Include("Product").Where(x => x.IsDelete == false && x.UserID == member).ToList();
                foreach (var item in bList)
                {
                    item.IsDelete = true;
                }
            }

            var rt = db.SaveChanges();
            return Json(rt, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AddProduct(Guid productID, int quantity)
        {
            var product = db.Products.Where(x => x.ID == productID).FirstOrDefault();

            if (Request.Cookies["Member"] == null)
            {
        

                var hUser = new Guid(Request.Cookies["HostUser"].Values["uID"]);

                var pBasket = db.Baskets.Where(x => x.HostUserID == hUser && x.IsDelete == false && x.ProductID == productID && x.UserID == null).FirstOrDefault();
  

                if (pBasket == null)
                    {

                        if (product.Stock >= quantity)
                        {
                            db.Baskets.Add(new TatliGezi.Models.Shop.Basket
                            {
                                ID = Guid.NewGuid(),
                                AddDate = DateTime.Now,
                                UpdateDate = DateTime.Now,
                                ProductID = productID,
                                Quantity = quantity,
                                ProductQuantityPrice= product.ProductPrice * quantity,
                                ProductQuantityDiscountPrice= product.ProductDiscountPrice * quantity,
                                HostUserID = hUser,
                            });


                        }

                        else
                        {
                            ViewBag.Alert = "Stokta Ürün bulunmamaktadır.";


                        }
                    }
                    else
                    {
                        var totalQuantity= quantity + pBasket.Quantity;

                        if (product.Stock >= totalQuantity)
                        {
                            pBasket.Quantity += quantity;
                        pBasket.ProductQuantityPrice = product.ProductPrice * totalQuantity;
                        pBasket.ProductQuantityDiscountPrice = product.ProductDiscountPrice * totalQuantity;
                        }
                        else
                        {
                            ViewBag.Alert = "Stokta Ürün bulunmamaktadır.";


                        }
                    }
                
           


            }
            else
            {
                Guid member = new Guid(Request.Cookies["Member"].Values["uID"]);

                var pBasket = db.Baskets.Where(x => x.HostUserID == null && x.IsDelete == false && x.ProductID == productID && x.UserID == member).FirstOrDefault();

      

                if (pBasket == null)
                {
                    if (product.Stock >= quantity)
                    {
                        db.Baskets.Add(new TatliGezi.Models.Shop.Basket
                        {
                            ID = Guid.NewGuid(),
                            AddDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            ProductID = productID,
                            Quantity = quantity,
                            ProductQuantityPrice = product.ProductPrice * quantity,
                            ProductQuantityDiscountPrice = product.ProductDiscountPrice * quantity,
                            UserID = member,

                  
                    });

                    }
                    else
                    {
                        ViewBag.Alert = "Stokta Ürün bulunmamaktadır.";


                    }
                }
                else
                {
                    var totalQuantity = quantity + pBasket.Quantity;

                    if (product.Stock >= totalQuantity)
                    {
                        pBasket.Quantity += quantity;
                        pBasket.ProductQuantityPrice = product.ProductPrice * totalQuantity;
                        pBasket.ProductQuantityDiscountPrice = product.ProductDiscountPrice * totalQuantity;
                    }
                    else
                    {
                        ViewBag.Alert = "Stokta Ürün bulunmamaktadır.";


                    }
                }
            }

            var rt = db.SaveChanges();

            return Json(new { rt, ViewBag.Alert }, JsonRequestBehavior.AllowGet);
        }

        [Route("sepetim")]
        public ActionResult Index(string searchString, int? page)
        {

            SomeCommonMethod();

            if (!String.IsNullOrEmpty(searchString))
            {

                return Redirect("/arama/" + searchString);

            }

            else
            {
                if (Request.Cookies["Member"] == null)
                {
                    Guid hUser = new Guid(Request.Cookies["HostUser"].Values["uID"]);

                    List<Basket> bList = db.Baskets.Include("Product").Where(x => x.IsDelete == false && x.HostUserID == hUser).OrderByDescending(x => x.AddDate).ToList();

                    var basketListStandart = db.Baskets.Include("Product").Where(x => x.IsDelete == false && x.HostUserID == hUser && x.Product.Discount == 0).ToList(); //indirimsiz ürünlerin listesi

                    var totalStandart = basketListStandart.Sum(x => x.ProductQuantityPrice); //indirimsiz ürünlerin fiyat toplamı

                    var basketListDiscount = db.Baskets.Include("Product").Where(x => x.IsDelete == false && x.HostUserID == hUser && x.Product.Discount != 0).ToList(); //indirimli ürünlerin listesi

                    var totalDiscount = basketListDiscount.Sum(x => x.ProductQuantityDiscountPrice); //indirimli ürünlerin fiyat toplamı

                    ViewData["TotalPrice"] = (totalStandart + totalDiscount).ToString("##.##");

                    var cargo = db.Cargos.Where(x => x.IsDelete == false).FirstOrDefault();

                    ViewData["CargoPrice"] = cargo.Price;

                    ViewData["TotalPayment"] = ((totalStandart + totalDiscount) + cargo.Price).ToString("##.##");

                    return View(bList);
                }
                else
                {
                    Guid member = new Guid(Request.Cookies["Member"].Values["uID"]);

                    List<Basket> bList = db.Baskets.Include("Product").Where(x => x.IsDelete == false && x.UserID == member).OrderByDescending(x => x.AddDate).ToList();

                    var basketListStandart = db.Baskets.Include("Product").Where(x => x.IsDelete == false && x.UserID == member && x.Product.Discount == 0).ToList(); //indirimsiz ürünlerin listesi

                    var totalStandart = basketListStandart.Sum(x => x.ProductQuantityPrice); //indirimsiz ürünlerin fiyat toplamı

                    var basketListDiscount = db.Baskets.Include("Product").Where(x => x.IsDelete == false && x.UserID == member && x.Product.Discount != 0).ToList(); //indirimli ürünlerin listesi

                    var totalDiscount = basketListDiscount.Sum(x => x.ProductQuantityDiscountPrice); //indirimli ürünlerin fiyat toplamı

                    ViewData["TotalPrice"] = (totalStandart + totalDiscount).ToString("##.##");

                    var cargo = db.Cargos.Where(x => x.IsDelete == false).FirstOrDefault();

                    ViewData["CargoPrice"] = cargo.Price;

                    ViewData["TotalPayment"] = ((totalStandart + totalDiscount) + cargo.Price).ToString("##.##");

                    var adress = db.UserAdresses.Where(x => x.IsDelete == false && x.UserID == member).FirstOrDefault();

                    if (adress != null)
                    {
                        ViewData["Adress"] = adress.ID;
                        return View(bList);

                    }
                    else
                    {
                        return View(bList);
                    }

                }
            }
        }


 
        public JsonResult Delete(Guid id)
        {
            var del = db.Baskets.Find(id);
            del.IsDelete = true;
            db.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }
    }
}