using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TatliGezi.Models;

namespace TatliGezi.Areas.Admin.Controllers
{
    public class GalleryController : BaseAdminController
    {
        // GET: Admin/Gallery
        public ActionResult Index()
        {
            List<Gallery> gList = db.Galleries.Where(x => x.IsDelete == false).ToList();

            if (Request.Cookies["Admin"] != null)
            {
                return View(gList);
            }
            else
            {
                return Redirect("/Admin/LoginAdmin/Index/");
            }
        }
        public ActionResult GalleryAdd()
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
        public ActionResult GalleryAdd(Gallery gallery, HttpPostedFileBase resim)
        {
            if (ModelState.IsValid)
            {
                if (resim != null)
                {
                    var fileName = DateTime.Now.ToString("yyMMddHHmmssff") + resim.FileName;
                    var path = Server.MapPath("/Images/Gallery/" + fileName);
                    resim.SaveAs(path);
                    gallery.PhotoPath = "/Images/Gallery" + fileName;
                }

                gallery.AddDate = DateTime.Now;
                gallery.UpdateDate = DateTime.Now;
                gallery.IsSlider = false;
                db.Galleries.Add(gallery);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Durum = -1;
                return View();
            }

        }

        public ActionResult GalleryEdit(int id)
        {
            Gallery gallery = db.Galleries.Find(id);
            if (Request.Cookies["Admin"] != null)
            {
                return View(gallery);
            }
            else
            {
                return Redirect("/Admin/LoginAdmin/Index/");
            }
        }

        [HttpPost]
        public ActionResult GalleryEdit(Gallery gallery, HttpPostedFileBase resim)
        {
            if (ModelState.IsValid)
            {
                if (resim != null)
                {
                    var fileName = DateTime.Now.ToString("yyMMddHHmmssff") + resim.FileName;
                    var path = Server.MapPath("/Images/Gallery/" + fileName);
                    resim.SaveAs(path);
                    gallery.PhotoPath = "/Images/Gallery/" + fileName;
                }

                var _gallery = db.Galleries.Find(gallery.ID);
                gallery.AddDate = _gallery.AddDate;
                gallery.UpdateDate = DateTime.Now;
                gallery.IsDelete = _gallery.IsDelete;
                if (gallery.PhotoPath != null)
                {
                    _gallery.PhotoPath = gallery.PhotoPath;
                }
                db.Entry(_gallery).CurrentValues.SetValues(gallery);
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
            var del = db.Galleries.Find(id);
            del.IsDelete = true;
            db.SaveChanges();
            return Json("");

        }
        public JsonResult Slider(int id)
        {
            var del = db.Galleries.Find(id);
            del.IsSlider = true;
            db.SaveChanges();
            return Json("");

        }
        public JsonResult SliderDel(int id)
        {
            var del = db.Galleries.Find(id);
            del.IsSlider = false;
            db.SaveChanges();
            return Json("");

        }
    }
}