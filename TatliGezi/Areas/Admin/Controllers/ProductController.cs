using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TatliGezi.Models.Shop;
using TatliGezi.SeoSetting;

namespace TatliGezi.Areas.Admin.Controllers
{
    public class ProductController : BaseAdminController
    {
        // GET: Admin/Product
        void drpDoldur()
        {
            List<ProductCategory> cList = db.ProductCategories.Where(x => x.IsDelete == false).ToList();
            List<SelectListItem> sList = cList.Select(x => new SelectListItem()
            {
                Text = x.CategoryName,
                Value = x.ID.ToString()
            }).ToList();
            ViewData["Category"] = sList;
        }
        void drpDoldurEdit(Guid? id)
        {
            List<ProductCategory> cList = db.ProductCategories.Where(x => x.IsDelete == false).ToList();
            List<SelectListItem> sList = cList.Select(x => new SelectListItem()

            {
                Text = x.CategoryName,
                Value = x.ID.ToString()


            }).ToList();
            foreach (var item in sList)
            {
                if (item.Value == id.ToString())
                {
                    item.Selected = true;
                }


            }
            ViewData["Category"] = sList;
        }

        void drpDoldurKdv()
        {
            List<KDVRate> kList = db.KDVRates.Where(x => x.IsDelete == false).ToList();
            List<SelectListItem> skList = kList.Select(x => new SelectListItem()
            {
                Text = x.Rate.ToString(),
                Value = x.ID.ToString()
            }).ToList();
            ViewData["KDVRate"] = skList;
        }
        void drpDoldurKdvEdit(Guid? id)
        {
            List<KDVRate> kList = db.KDVRates.Where(x => x.IsDelete == false).ToList();
            List<SelectListItem> skList = kList.Select(x => new SelectListItem()

            {
                Text = x.Rate.ToString(),
                Value = x.ID.ToString()


            }).ToList();
            foreach (var item in skList)
            {
                if (item.Value == id.ToString())
                {
                    item.Selected = true;
                }


            }
            ViewData["KDVRate"] = skList;
        }
        public ActionResult Index()
        {
            List<Product> cList = db.Products.Where(x => x.IsDelete == false).ToList();
            foreach (var item in cList)
            {
                var imageView = db.ProductImages.Where(x => x.ProductID == item.ID).OrderBy(x=> x.AddDate).First().PhotoPath;
                item.ImageView = imageView;
                db.SaveChanges();

            }
            if (Request.Cookies["Admin"] != null)
            {
                return View(cList);
            }
            else
            {
                return Redirect("/Admin/LoginAdmin/Index/");
            }
        }
        public ActionResult ProductAdd()
        {
            if (Request.Cookies["Admin"] != null)
            {
                drpDoldur();
                drpDoldurKdv();

                return View();
            }
            else
            {
                return Redirect("/Admin/LoginAdmin/Index/");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ProductAdd(Product product, string[] CategoryList, string[] KDVRateList)
        {
            drpDoldur();
            drpDoldurKdv();

            if (ModelState.IsValid)
            {

                var product1 = db.Products.Where(x => x.ProductName == product.ProductName).FirstOrDefault();
                if (product1 != null)
                {
                    ViewBag.Durum = 0;
                    return View();
                }


                foreach (var item in CategoryList)
                {
                    product.ProductCategoryID = new Guid(item);
                }

                foreach (var item in KDVRateList)
                {
                    product.KdvRateID = new Guid(item);

                }

                var kdv = db.KDVRates.Where(x => x.ID == product.KdvRateID).FirstOrDefault().Rate;
                product.ID = Guid.NewGuid();
                product.AddDate = DateTime.Now;
                product.UpdateDate = DateTime.Now;
                product.IsDelete = false;
                product.ProductPrice = product.Price + (product.Price * Convert.ToInt32(kdv) / 100);
                product.ProductDiscountPrice = product.ProductPrice - (product.ProductPrice * product.Discount / 100);

                db.Products.Add(product);
                db.SaveChanges();
                Session["ProductID"] = product.ID.ToString();

                return RedirectToAction("ProductImage", new { ID = Session["ProductID"] });

            }
            else
            {
                ViewBag.Durum = -1;
                return View();
            }

        }

        public ActionResult ProductImage(Guid id)
        {
            Product product = db.Products.Find(id);

            if (Request.Cookies["Admin"] != null)
            {
                drpDoldurKdvEdit(product.ProductCategoryID);
                return View(product);

            }
            else
            {
                return Redirect("/Admin/LoginAdmin/Index/");
            }

        }

        [HttpPost]
        public JsonResult ProductImageDetail()
        {
            string _result = "";
            var pid = new Guid((string)Session["ProductID"]);
            var product = db.Products.Find(pid);
            var imageName = Seo.EditAdress(product.ProductName);

            try
            {
                string[] _Keys = Request.Files.AllKeys;
                //if (Request.Files.Count > 0)
                //{
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var image = new ProductImage();

                    string _ResimAdi = Request.Files[i].FileName;

                    string _Description = _Keys[i];

                    if (_ResimAdi == "NoImage.PNG")
                    {
                        //veri tabanı kaydı yapacak resim adı null olacak
                    }
                    else
                    {
                        image.ID = Guid.NewGuid();
                        image.ProductID = pid;
                       
                        var file = Request.Files[i];
                        //var _FileName = DateTime.Now.ToString("yyMMddHHmmssff") + file.FileName;
                        var _FileName = imageName + file.FileName;
                        var path = Server.MapPath("/Images/Shop/" + _FileName);
                        file.SaveAs(path);           
                        image.PhotoPath = "/Images/Shop/" + _FileName;

                        image.ImageDescription = _Description;
                        image.AddDate = DateTime.Now;
                        image.UpdateDate = DateTime.Now;
                        image.IsDelete = false;
                        db.ProductImages.Add(image);
                        db.SaveChanges();


                    }
                }
           
                    var imageView = db.ProductImages.Where(x => x.ProductID == pid).OrderBy(x => x.AddDate).First().PhotoPath;

                    product.ImageView = imageView;
                    db.SaveChanges();
               
            }

            catch (Exception err)
            {
                _result = err.ToString();
            }



            drpDoldur();

            return Json(new { id = Session["ProductID"] });
        }

        public ActionResult ProductImageEdit(Guid id)
        {
            Product product = db.Products.Find(id);
           List<ProductImage> imageList = db.ProductImages.Where(x => x.ProductID == id).ToList(); 

            if (Request.Cookies["Admin"] != null)
            {
                Session["ProductID"] = product.ID.ToString();

                return View(imageList);

            }
            else
            {
                return Redirect("/Admin/LoginAdmin/Index/");
            }

        }

        [HttpPost]
        public ActionResult ProductImageEdit()
        {
            string _result = "";
            var pid = new Guid((string)Session["ProductID"]);
            var productDB = db.Products.Find(pid);
            var imageName = Seo.EditAdress(productDB.ProductName);

            try
            {
                string[] _Keys = Request.Files.AllKeys;
             
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    Guid iID = Guid.Parse(_Keys[i]);

                    var imgID = db.ProductImages.Find(iID);



                    string _ResimAdi = Request.Files[i].FileName;
                    
                    if (_ResimAdi == "NoImage.PNG")
                    {
                        //veri tabanı kaydı yapacak resim adı null olacak
                    }
                    else
                    {
                        var file = Request.Files[i];
               
                        var _FileName = imageName + file.FileName;
                        var path = Server.MapPath("/Images/Shop/" + _FileName);
                        file.SaveAs(path);
                        imgID.PhotoPath = "/Images/Shop/" + _FileName;

                        db.SaveChanges();

                        }
               
                }
                var product = db.Products.ToList();
                foreach (var item in product)
                {
                    var imageView = db.ProductImages.Where(x => x.ProductID == item.ID).OrderBy(x => x.AddDate).First().PhotoPath;

                    item.ImageView = imageView;
                    db.SaveChanges();
                }
            }

            catch (Exception err)
            {
                _result = err.ToString();
            }



            drpDoldur();

            return Json(new { id = Session["ProductID"] });
        }

        public ActionResult ProductEdit(Guid id)
        {
            Product product = db.Products.Find(id);

            if (Request.Cookies["Admin"] != null)
            {
                drpDoldurEdit(product.ProductCategoryID);
                drpDoldurKdvEdit(product.KdvRateID);
                return View(product);
            }
            else
            {
                return Redirect("/Admin/LoginAdmin/Index/");
            }
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult ProductEdit(Product product,  string[] CategoryList, string[] KDVRateList)
        {
            if (ModelState.IsValid)
            {
                  var dbProduct = db.Products.Find(product.ID);

                foreach (var item in CategoryList)
                {
                    product.ProductCategoryID =Guid.Parse(item);
                }

                foreach (var item in KDVRateList)
                {
                    product.KdvRateID = Guid.Parse(item);
                }
                var kdv = db.KDVRates.Where(x => x.ID == product.KdvRateID).FirstOrDefault().Rate;

                product.AddDate = dbProduct.AddDate;
                product.UpdateDate = DateTime.Now;
                product.IsDelete = false;

                product.ProductPrice = product.Price + (product.Price * Convert.ToInt32(kdv) / 100);
                product.ProductDiscountPrice = product.ProductPrice - (product.ProductPrice * product.Discount / 100);

                db.Entry(dbProduct).CurrentValues.SetValues(product);
               db.SaveChanges();
               return RedirectToAction("Index");

                //Session["ProductID"] = product.ID.ToString();

                //return RedirectToAction("ProductImage", new { ID = Session["ProductID"] });

            }
            //if (ModelState.IsValid)
            //{
            //    var _product = db.Products.Find(product.ID);
            //    product.AddDate = _product.AddDate;
            //    product.UpdateDate = DateTime.Now;
            //    product.IsDelete = _product.IsDelete;


            //    db.Entry(_product).CurrentValues.SetValues(product);
            //    db.SaveChanges();
            //    foreach (var item in CategoryList)
            //    {
            //        if (item != _product.ProductCategoryID.ToString())
            //            _product.ProductCategoryID = new Guid(item);
            //    }
            //    return RedirectToAction("Index");
            //}
            else
            {
                ViewBag.Durum = -1;
                return View();

            }
        }

        public JsonResult Delete(int id)
        {
            var del = db.Products.Find(id);
            del.IsDelete = true;
            db.SaveChanges();
            return Json("");

        }

  
    }
}