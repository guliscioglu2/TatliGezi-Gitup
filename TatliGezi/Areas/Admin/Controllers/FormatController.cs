using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TatliGezi.Models;

namespace TatliGezi.Areas.Admin.Controllers
{
    public class FormatController : BaseAdminController
    {
        // GET: Admin/Format
               public ActionResult Index()
        {
            List<Format> fList = db.Formats.ToList();

            if (Request.Cookies["Admin"] != null)
            {
                return View(fList);
            }
            else
            {
                return Redirect("/Admin/LoginAdmin/Index/");
            }
        }
        public ActionResult FormatAdd()
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
        public ActionResult FormatAdd(Format format)
        {
            if (ModelState.IsValid)
            {
                var format1 = db.Formats.Where(x => x.FormatName == format.FormatName).FirstOrDefault();
                if (format1 != null)
                {
                    ViewBag.Durum = 0;
                    return View();
                }

                db.Formats.Add(format);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Durum = -1;
                return View();
            }

        }

        public ActionResult FormatEdit(int id)
        {
            Format format = db.Formats.Find(id);
            if (Request.Cookies["Admin"] != null)
            {
                return View(format);
            }
            else
            {
                return Redirect("/Admin/LoginAdmin/Index/");
            }
        }

        [HttpPost]
        public ActionResult FormatEdit(Format format)
        {
            if (ModelState.IsValid)
            {
                var format1 = db.Formats.Where(x => x.FormatName == format.FormatName).FirstOrDefault();
                if (format1 != null)
                {
                    ViewBag.Durum = 0;
                    return View();
                }
                var _format = db.Formats.Find(format.ID);
              
                format.IsDelete = _format.IsDelete;
                db.Entry(_format).CurrentValues.SetValues(format);
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
            var del = db.Formats.Find(id);
            del.IsDelete = true;
            db.SaveChanges();
            return Json("");

        }
    }
}