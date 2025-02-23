using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TatliGezi.Models.Shop;

namespace TatliGezi.Areas.Admin.Controllers
{
    public class OrderAdminController : BaseAdminController
    {
        // GET: Admin/OrderAdmin
        public ActionResult OrderList()
        {
            List<Order> oList = db.Orders.Include("Status").Where(x => x.IsDelete == false ).ToList();
            var orderProducts = db.OrderProducts.Include("Product").ToList();
            //var products = db.Products.ToList();

            return View(oList);
        }
    }
}