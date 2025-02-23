using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TatliGezi.Models;
using TatliGezi.Models.Shop;
using System.Threading.Tasks;

namespace TatliGezi.Areas.Admin.Controllers
{
    public class ProductCategoryController : BaseAdminController
    {
        // GET: Admin/ProductCategory
        public ActionResult Index()
        {
            List<ProductCategory> cList = db.ProductCategories.Where(x => x.IsDelete == false).ToList();

            if (Request.Cookies["Admin"] != null)
            {
                return View(cList);
            }
            else
            {
                return Redirect("/Admin/LoginAdmin/Index/");
            }
        }
        public ActionResult CategoryAdd()
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
        public ActionResult CategoryAdd(ProductCategory category)
        {
            if (ModelState.IsValid)
            {
                var category1 = db.ProductCategories.Where(x => x.CategoryName == category.CategoryName).FirstOrDefault();
                if (category1 != null)
                {
                    ViewBag.Durum = 0;
                    return View();
                }
                category.ID = Guid.NewGuid();
                category.AddDate = DateTime.Now;
                category.UpdateDate = DateTime.Now;
                db.ProductCategories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Durum = -1;
                return View();
            }

        }

        public ActionResult CategoryEdit(Guid id)
        {
            ProductCategory category = db.ProductCategories.Find(id);
            if (Request.Cookies["Admin"] != null)
            {
                return View(category);
            }
            else
            {
                return Redirect("/Admin/LoginAdmin/Index/");
            }
        }

        [HttpPost]
        public ActionResult CategoryEdit(ProductCategory category)
        {
            if (ModelState.IsValid)
            {
                var category1 = db.ProductCategories.Where(x => x.CategoryName == category.CategoryName).FirstOrDefault();
                if (category1 != null)
                {
                    ViewBag.Durum = 0;
                    return View();
                }
                var _category = db.ProductCategories.Find(category.ID);
                category.AddDate = _category.AddDate;
                category.UpdateDate = DateTime.Now;
                category.IsDelete = _category.IsDelete;
                db.Entry(_category).CurrentValues.SetValues(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Durum = -1;
                return View();

            }
        }

        public JsonResult Delete(Guid id)
        {
            var del = db.ProductCategories.Find(id);
            del.IsDelete = true;
            db.SaveChanges();
            return Json("");

        }

        //public JsonResult Active(int id)
        //{
        //    var del = db.Categories.Find(id);
        //    del.IsActive = true;
        //    db.SaveChanges();
        //    return Json("");

        //}
    }
}