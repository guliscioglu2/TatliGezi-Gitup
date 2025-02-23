using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TatliGezi.Models;

namespace TatliGezi.Areas.Admin.Controllers
{
    public class SliderController : BaseAdminController
    {
        // GET: Admin/Slider
        public ActionResult Index()
        {
            List<Slider> cList = db.Sliders.Where(x => x.IsDelete == false).ToList();

            if (Request.Cookies["Admin"] != null)
            {
                return View(cList);
            }
            else
            {
                return Redirect("/Admin/LoginAdmin/Index/");
            }
        }
        public ActionResult SliderAdd()
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
        public ActionResult SliderAdd(Slider slider, HttpPostedFileBase resim )
        {
            if (ModelState.IsValid)
            {
                var slider1 = db.Sliders.Where(x => x.Content == slider.Content).FirstOrDefault();
                if (slider1 != null)
                {
                    ViewBag.Durum = 0;
                    return View();
                }
                if (resim != null)
                {
                    var fileName = DateTime.Now.ToString("yyMMddHHmmssff") + resim.FileName;
                    var path = Server.MapPath("/Images/Slider/" + fileName);
                    resim.SaveAs(path);
                    slider.PhotoPath = "/Images/Slider/" + fileName;
                }

                slider.ID = Guid.NewGuid();
                slider.AddDate = DateTime.Now;
                slider.UpdateDate = DateTime.Now;
                db.Sliders.Add(slider);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Durum = -1;
                return View();
            }

        }

        public ActionResult SliderEdit(Guid id)
        {
            Slider slider = db.Sliders.Find(id);
            if (Request.Cookies["Admin"] != null)
            {
                return View(slider);
            }
            else
            {
                return Redirect("/Admin/LoginAdmin/Index/");
            }
        }

        [HttpPost]
        public ActionResult SliderEdit(Slider slider)
        {
            if (ModelState.IsValid)
            {
                var slider1 = db.Sliders.Where(x => x.Content == slider.Content).FirstOrDefault();
                if (slider1 != null)
                {
                    ViewBag.Durum = 0;
                    return View();
                }
                var _slider = db.Sliders.Find(slider.ID);
                slider.AddDate = _slider.AddDate;
                slider.UpdateDate = DateTime.Now;
                slider.IsDelete = _slider.IsDelete;
                db.Entry(_slider).CurrentValues.SetValues(slider);
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
            var del = db.Sliders.Find(id);
            del.IsDelete = true;
            db.SaveChanges();
            return Json("");

        }

     
    }
}