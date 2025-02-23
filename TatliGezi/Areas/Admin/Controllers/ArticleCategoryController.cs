using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TatliGezi.Models;

namespace TatliGezi.Areas.Admin.Controllers
{
    public class ArticleCategoryController :  BaseAdminController
    {
        // GET: Admin/ArticleCategory
        public ActionResult Index()
        {
            List<ArticleCategory> cList = db.ArticleCategories.Where(x => x.IsDelete == false).ToList();

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
        public ActionResult CategoryAdd(ArticleCategory category)
        {
            if (ModelState.IsValid)
            {
                var category1 = db.ArticleCategories.Where(x => x.CategoryName == category.CategoryName).FirstOrDefault();
                if (category1 != null)
                {
                    ViewBag.Durum = 0;
                    return View();
                }
                category.ID = Guid.NewGuid();
                category.AddDate = DateTime.Now;
                category.UpdateDate = DateTime.Now;
                db.ArticleCategories.Add(category);
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
            ArticleCategory category = db.ArticleCategories.Find(id);
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
        public ActionResult CategoryEdit(ArticleCategory category)
        {
            if (ModelState.IsValid)
            {
                var dbCategory = db.ArticleCategories.Find(category.ID);

                category.AddDate = dbCategory.AddDate;
                category.UpdateDate = DateTime.Now;
                category.IsDelete = dbCategory.IsDelete;
                db.Entry(dbCategory).CurrentValues.SetValues(category);
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
            var del = db.ArticleCategories.Find(id);
            del.IsDelete = true;
            db.SaveChanges();
            return Json("");

        }

        public JsonResult Active(Guid id)
        {
            var del = db.ArticleCategories.Find(id);
            del.IsActive = true;
            db.SaveChanges();
            return Json("");

        }
    }
}