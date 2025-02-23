using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TatliGezi.Models.Shop;

namespace TatliGezi.Areas.Admin.Controllers
{
    public class HomeAdminController : BaseAdminController
    {
        // GET: Admin/HomeAdmin
        public ActionResult Index()
        {
            if (Request.Cookies["Admin"] != null)
            {

                List<Order> oList = db.Orders.Include("Status").Where(x => x.IsDelete == false && x.Status.Value == 2).ToList();
                ViewData["OrderCount"] = db.Orders.Include("Status").Where(x => x.IsDelete == false && x.Status.Value == 2).Count();
                return View();
            }
            else
            {
                return Redirect("/Admin/LoginAdmin/Index/");
            }
        }
    }
}