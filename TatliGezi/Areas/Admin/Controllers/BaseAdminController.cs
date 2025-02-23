using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TatliGezi.Context;

namespace TatliGezi.Areas.Admin.Controllers
{
    public class BaseAdminController : Controller
    {
        // GET: Admin/BaseAdmin
        protected TatliGeziContext db;
        public BaseAdminController()
        {
            db = new TatliGeziContext();
        }
    }
}