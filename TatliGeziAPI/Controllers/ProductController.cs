using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TatliGezi.Context;
using TatliGezi.Models.Shop;

namespace TatliGeziAPI.Controllers
{
    public class ProductController : ApiController
    {        public List<Product> Get()
        {
            var db = new TatliGeziContext();
            var data = db.Products.Where(x => x.IsDelete == false).ToList();
            return data;
        }
        public Product Get(Guid id)
        {
            var db = new TatliGeziContext();
            var data = db.Products.Where(x => x.ID == id).FirstOrDefault();
            return data;
        }

    }
}
