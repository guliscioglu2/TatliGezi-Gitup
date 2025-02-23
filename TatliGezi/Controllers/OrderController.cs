using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TatliGezi.Models;
using TatliGezi.Models.Shop;
using TatliGezi.DTO;
using System.Data.Entity.Core;
using TatliGezi.Models.MultipleData;
using Iyzipay;
using Iyzipay.Request;
using Iyzipay.Model;
using NUnit.Framework;
using System.Net.Mail;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Web.UI.WebControls;
using System.IO;
using System.Drawing.Imaging;

namespace TatliGezi.Controllers
{
    public class OrderController : BaseController
    {
        // GET: Order
        int pageSize = 7;
        void drpDoldurCity()
        {
            List<City> cList = db.Cities.ToList();
            List<SelectListItem> sList = cList.Select(x => new SelectListItem()
            {
                Text = x.CityName,
                Value = x.ID.ToString()
            }).ToList();
            ViewData["City"] = sList;
        }

        void drpDoldurTown()
        {

            List<String> tList = new List<String>();
            tList.Add("İlçe Seçiniz");

            List<SelectListItem> sList = tList.Select(x => new SelectListItem()
            {
                Text = "İlçe Seçiniz",
                Value = "0",
            }).ToList();
            ViewData["Town"] = sList;
        }

        [HttpPost]
        public JsonResult AdressSave(Guid ID)
        {

            var adress = db.UserAdresses.Find(ID);
            adress.AddDate = DateTime.Now;

            var rt = db.SaveChanges();
            return Json(rt, JsonRequestBehavior.AllowGet);
        }

        [Route("adresbilgisi")]
        public ActionResult SelectedAdres()
        {
            SomeCommonMethod();

            SomePaymentMethod();

            Guid member = new Guid(Request.Cookies["Member"].Values["uID"]);

            var adress = db.UserAdresses.Include("User").Where(x => x.UserID == member).OrderByDescending(x => x.AddDate).FirstOrDefault();

            return View(adress);
        }

        [Route("adreslistesi")]
        public ActionResult AdressList()
        {
            SomeCommonMethod();
            SomePaymentMethod();

            Guid member = new Guid(Request.Cookies["Member"].Values["uID"]);

            List<UserAdress> aList = db.UserAdresses.Where(x => x.UserID == member).OrderByDescending(x => x.AddDate).ToList();

            return View(aList);
        }
        [Route("adresekle")]
        public ActionResult AddAdress(string searchString, int? page)
        {

            SomeCommonMethod();

            //var arts = from m in db.Articles
            //           select m;

            if (!String.IsNullOrEmpty(searchString))
            {

                return Redirect("/arama/" + searchString);

            }
            else
            {
                return View();

            }
        }

        [Route("adresekle")]
        [HttpPost]
        public ActionResult AddAdress(UserAdress adress, string myCityselect, string myselect)
        {
            Guid member = new Guid(Request.Cookies["Member"].Values["uID"]);

            adress.ID = Guid.NewGuid();
            adress.AddDate = DateTime.Now;
            adress.IsDelete = false;
            adress.UpdateDate = DateTime.Now;
            adress.UserID = member;
            adress.TownID = new Guid(myselect);
            adress.CityID = new Guid(myCityselect);

            db.UserAdresses.Add(adress);
            db.SaveChanges();
            return Redirect("AdresBilgisi");
        }

        public ActionResult AddOrder(Guid id)
        {
            Random rnd = new Random();

            Guid member = new Guid(Request.Cookies["Member"].Values["uID"]);

            var kdvList = db.KDVRates.Where(x => x.IsDelete == false).ToList();

            Order order = new Order();
            order.ID = Guid.NewGuid();
            order.OrderCode = rnd.Next(100000000, 999999999);
            order.AddDate = DateTime.Now;
            order.UpdateDate = DateTime.Now;
            order.IsDelete = false;
            order.UserID = member;
            order.StatusID = Guid.Parse("81fb6ed5-d669-469d-9408-091693d41c3c");

            var bList = db.Baskets.Include("Product").Where(x => x.IsDelete == false && x.UserID == member).ToList();

            foreach (var item in bList)
            {
                item.Product.Stock -= item.Quantity;
                db.SaveChanges();
            }


            var basketStandat = db.Baskets.Include("Product").Where(x => x.IsDelete == false && x.UserID == member && x.Product.Discount == 0).ToList();
            decimal totalStandart = basketStandat.Sum(x => x.ProductQuantityPrice); //indirimsiz ürünlerin fiyat toplamı

            var basketDiscount = db.Baskets.Include("Product").Where(x => x.IsDelete == false && x.UserID == member && x.Product.Discount != 0).ToList();
            decimal totalDiscount = basketDiscount.Sum(x => x.ProductQuantityDiscountPrice); //indirimli ürünlerin fiyat toplamı
            order.TotalPrice = totalStandart + totalDiscount;

            order.UserAdressID = id;

            order.OrderProducts = new List<OrderProduct>();

            foreach (var item in bList)
            {
                order.OrderProducts.Add(new OrderProduct
                {
                    ID = Guid.NewGuid(),
                    AddDate = DateTime.Now,
                    UserID = member,
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    ProductQuantityPrice = item.Product.ProductPrice * item.Quantity, // data ürürnün miktarıyla birlike fiyatı
                    ProductQuantityDiscountPrice = item.Product.ProductDiscountPrice * item.Quantity //data ürünün miktarıyla birlikte indirimli fiyatı

                });

                db.Baskets.Remove(item);
            }
            db.Orders.Add(order);
            db.SaveChanges();
            var order1 = db.Orders.Find(order.ID);

            return Redirect("/onay/" + order1.ID);
        }

        [Route("onay/{id}")]
        public ActionResult Confirmation(Guid id)
        {
            SomeCommonMethod();

            Guid member = new Guid(Request.Cookies["Member"].Values["uID"]);
            var user = db.Users.Find(member);

            var city = db.Cities.ToList();
            var town = db.Towns.ToList();

            var order = db.Orders.Include("OrderProducts")
                .Include("OrderProducts.Product")
                .Include("OrderPayments")
                .Include("Status")
                .Include("UserAdress")
                .Where(x => x.ID == id).FirstOrDefault();


            var fromEmail = new MailAddress(user.Email);
            var toEmail = new MailAddress("order@tatligezi.com");
            string subject = "";
            string body="";

            foreach (var item in order.OrderProducts)
            {

                body = "<body>"
             + "Sipariş bilgileri alttadır."
                          + "<br/><br/>"

             + "Ürün Adı : " + item.Product.ProductName
             + "<br/><br/>"
             + "Ürün Resmi : " + @"<img src='https://localhost:44333/" + item.Product.ImageView + @"'/>"
             //+ "Ürün Resmi : " + @"<img src='cid:myImageID" + @"'/>"
               //+  @"<img src=""cid:{0}"" />", inline.ContentId
               // + "<br/><br/>"

             + "</body>";
             
            }
          
            subject = "Yeni sipariş oluşturuldu!";
            
            SmtpClient smtpClient = new SmtpClient("mail.tatligezi.com", 587);
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new System.Net.NetworkCredential("order@tatligezi.com", "05359579170ZGul");
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.Port = 587;
            MailMessage message1 = new MailMessage();

            try
            {

                MailAddress fromAddress = new MailAddress(fromEmail.ToString(), toEmail.ToString());

                smtpClient.Host = "mail.tatligezi.com";
                message1.From = fromEmail;
                message1.To.Add(toEmail.ToString());
                message1.Subject = subject;
                message1.IsBodyHtml = true;
                message1.Body = body;

                string htmlBody = "<html><body><h1>Picture</h1><br><img src=\"cid:filename\"></body></html>";

                AlternateView avHtml = AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html);
                foreach (var item in order.OrderProducts)
                {
                    var filename = Convert.FromBase64String("/9j/4AAQSkZJRgABAgEASABIAAD/2wBDAAEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEB");

                    //var filename = Convert.FromBase64String("https://localhost:44333/Images/Shop/deneme1.jpg");

                    //var filename = Convert.FromBase64String("https://" + System.Web.HttpContext.Current.Request.Url.Authority + item.Product.ImageView/*, "image/jpeg"*/);
                    var contentId = item.Product.ImageView;

                    LinkedResource inline = new LinkedResource(new MemoryStream(filename), "image/jpeg");
                    inline.ContentId = contentId;
                    inline.TransferEncoding = TransferEncoding.Base64;

                    body = string.Format("<img src=\"cid:https://localhost:44333/Images/Shop/deneme1.jpg\" />", contentId);
                    var htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                    htmlView.LinkedResources.Add(inline);
                    //avHtml.LinkedResources.Add(inline);

                    //MailMessage mail = new MailMessage();
                    //mail.AlternateViews.Add(avHtml);

                    //Attachment att = new Attachment(filename);
                    //att.ContentDisposition.Inline = true;

                    //mail.From = fromEmail;
                    //mail.To.Add(toEmail.ToString());
                    //mail.Subject = "Client: " + data.client_id + " Has Sent You A Screenshot";
                    //mail.Body = String.Format(
                    //            @"<img src=""cid:{0}"" />", inline.ContentId);

                    //mail.IsBodyHtml = true;
                    //mail.Attachments.Add(att); 

                    //LinkedResource theEmailImage = new LinkedResource("https://" + System.Web.HttpContext.Current.Request.Url.Authority + item.Product.ImageView, MediaTypeNames.Image.Jpeg);
                    //theEmailImage.ContentId = Guid.NewGuid().ToString();

                    ////Add the Image to the Alternate view
                    //htmlView.LinkedResources.Add(theEmailImage);
                    //MailMessage mail = new MailMessage();

                    ////Add view to the Email Message
                    //message1.AlternateViews.Add(htmlView);


                    //LinkedResource res = new LinkedResource(item.Product.ImageView);
                    //res.ContentId = Guid.NewGuid().ToString();
                    //string htmlBody = @"<img src='cid:" + res.ContentId + @"'/>";
                    //AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
                    //alternateView.LinkedResources.Add(res);

                    //string attachmentPath = "https://" + System.Web.HttpContext.Current.Request.Url.Authority + item.Product.ImageView;

                    //Attachment inline = new Attachment(attachmentPath);
                    //inline.ContentDisposition.Inline = true;
                    //inline.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                    //inline.ContentId = Guid.NewGuid().ToString();
                    //inline.ContentType.MediaType = "image/png";
                    //inline.ContentType.Name = Path.GetFileName(attachmentPath);

                    //message1.Attachments.Add(inline);

                }

                using (var message = new MailMessage(fromEmail, toEmail)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })

                    smtpClient.Send(message1);

            }
            catch (Exception exp)
            {
                Response.Write(exp);

            }

            ViewData["OrderProduct"] = db.OrderProducts.Where(x => x.OrderID == id).Take(4).ToList();
            return View(order);
        }

        [Route("ret/{id}")]
        public ActionResult Denial(Guid id)
        {
            SomeCommonMethod();

            //Guid member = new Guid(Request.Cookies["Member"].Values["uID"]);
            //var city = db.Cities.ToList();
            //var town = db.Towns.ToList();

            var data = db.UserAdresses.Where(x => x.ID == id).FirstOrDefault();
            return View(data);
        }

        [Route("siparislerim")]
        public ActionResult Index(string searchString, int? page)
        {
            SomeCommonMethod();

            if (!String.IsNullOrEmpty(searchString))
            {

                return Redirect("/arama/" + searchString);

            }
            else
            {

                Guid member = new Guid(Request.Cookies["Member"].Values["uID"]);
                List<Order> oList = db.Orders.Include("Status").Where(x => x.IsDelete == false && x.UserID == member).OrderByDescending(x => x.AddDate).ToList();

                var orderProducts = db.OrderProducts.Include("Product").OrderByDescending(x => x.AddDate).ToList();
                return View(oList);

            }
        }

        [Route("siparisbilgisi/{id}")]
        public ActionResult Detail(Guid id)
        {
            SomeCommonMethod();

            Guid member = new Guid(Request.Cookies["Member"].Values["uID"]);
            var city = db.Cities.ToList();
            var town = db.Towns.ToList();

            var data = db.Orders.Include("OrderProducts")
                .Include("OrderProducts.Product")
                .Include("OrderPayments")
                .Include("Status")
                .Include("UserAdress")
                .Where(x => x.ID == id).FirstOrDefault();
            return View(data);
        }

        [Route("odemebilgileri/{id}")]
        public ActionResult Payment(Guid id)
        {
            SomeCommonMethod();

            ViewData["ProductCategory"] = db.ProductCategories.Where(x => x.IsDelete == false).ToList();

            Guid member = new Guid(Request.Cookies["Member"].Values["uID"]);

            List<Basket> bList = db.Baskets.Include("Product").Where(x => x.IsDelete == false && x.UserID == member).OrderByDescending(x => x.AddDate).ToList();

            var basketStandat = db.Baskets.Include("Product").Where(x => x.IsDelete == false && x.UserID == member && x.Product.Discount == 0).ToList();
            decimal totalStandart = basketStandat.Sum(x => x.ProductQuantityPrice); //indirimsiz ürünlerin fiyat toplamı

            var basketDiscount = db.Baskets.Include("Product").Where(x => x.IsDelete == false && x.UserID == member && x.Product.Discount != 0).ToList();
            decimal totalDiscount = basketDiscount.Sum(x => x.ProductQuantityDiscountPrice); //indirimli ürünlerin fiyat toplamı

            ViewData["TotalPrice"] = (totalStandart + totalDiscount).ToString();

            var cargo = db.Cargos.Where(x => x.IsDelete == false).FirstOrDefault();

            ViewData["CargoPrice"] = cargo.Price;

            ViewData["TotalPayment"] = ((totalStandart + totalDiscount) + cargo.Price).ToString("##.##");

            decimal total = (totalStandart + totalDiscount) + cargo.Price;

            var city = db.Cities.ToList();
            var town = db.Towns.ToList();

            List<User> uList = db.Users.ToList();
            List<City> cList = db.Cities.ToList();
            List<ProductCategory> pcList = db.ProductCategories.ToList();
            var adress = db.UserAdresses.Find(id);
            Session["AdressID"] = adress.ID.ToString();

            HttpCookie myCookie = new HttpCookie("Adress");
            myCookie.Values["aID"] = Convert.ToString(adress.ID);
            myCookie.Expires = DateTime.Now.AddDays(1d);
            Response.Cookies.Add(myCookie);


            Options options = new Options();

            //options.ApiKey = "lMev3c6oqTeVtJqlrb6HED5JIK1f3Y6a";
            //options.SecretKey = "AP4f5cy6y4123Bw0Z9MGXKYf9B5li4J2";
            //options.BaseUrl = "https://api.iyzipay.com";

            options.ApiKey = "sandbox-Z92PD5nIy1dHFVLlk3vPhnjzYmC8f2MS";
            options.SecretKey = "sandbox-qrEHpoSgczPPnNJ7F0tcGGG7PcO9MEKU";
            options.BaseUrl = "https://sandbox-api.iyzipay.com";



            CreateCheckoutFormInitializeRequest request = new CreateCheckoutFormInitializeRequest();
            request.Locale = Locale.TR.ToString();
            //request.ConversationId = "123456789";
            request.Price = total.ToString().Replace(",", ".");
            request.PaidPrice = total.ToString().Replace(",", ".");
            request.Currency = Currency.TRY.ToString();

            //request.BasketId = "B67832";
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString();
            request.CallbackUrl = "https://" + System.Web.HttpContext.Current.Request.Url.Authority + "/kontrol?addresID=" + id;

            //request.CallbackUrl = "https://localhost:44333/sonuc";


            List<int> enabledInstallments = new List<int>();
            enabledInstallments.Add(2);
            enabledInstallments.Add(3);
            enabledInstallments.Add(6);
            enabledInstallments.Add(9);
            request.EnabledInstallments = enabledInstallments;

            Buyer buyer = new Buyer();
            buyer.Id = adress.User.ID.ToString();
            buyer.Name = adress.User.Name;
            buyer.Surname = adress.User.Surname;
            buyer.GsmNumber = adress.User.PhoneNumber;
            buyer.Email = adress.User.Email;
            buyer.IdentityNumber = "12345678911";
            buyer.LastLoginDate = "2013-04-21 15:12:09";
            buyer.RegistrationDate = "2013-04-21 15:12:09";
            buyer.RegistrationAddress = adress.City.CityName;
            buyer.Ip = "85.34.78.112";
            buyer.City = adress.City.CityName;
            buyer.Country = "Turkey";
            buyer.ZipCode = adress.PostCode.ToString();
            request.Buyer = buyer;

            Address shippingAddress = new Address();
            shippingAddress.ContactName = adress.User.FullName;
            shippingAddress.City = adress.City.CityName;
            shippingAddress.Country = "Turkey";
            shippingAddress.Description = adress.AdressDetail;
            shippingAddress.ZipCode = adress.PostCode.ToString();
            request.ShippingAddress = shippingAddress;

            Address billingAddress = new Address();
            billingAddress.ContactName = adress.User.FullName;
            billingAddress.City = adress.City.CityName;
            billingAddress.Country = "Turkey";
            billingAddress.Description = adress.AdressDetail;
            billingAddress.ZipCode = adress.PostCode.ToString();
            request.BillingAddress = billingAddress;

            List<BasketItem> basketItems = new List<BasketItem>();


            foreach (var item in bList)
            {
                BasketItem firstBasketItem = new BasketItem();
                firstBasketItem.Id = item.ID.ToString();
                firstBasketItem.Name = item.Product.ProductName;
                firstBasketItem.Category1 = item.Product.ProductCategory.CategoryName;
                //firstBasketItem.Category2 = "Ürün";
                firstBasketItem.ItemType = BasketItemType.PHYSICAL.ToString();

                if (item.Product.Discount == 0)
                {
                    firstBasketItem.Price = item.ProductQuantityPrice.ToString().Replace(",", ".");

                }
                else
                {
                    firstBasketItem.Price = item.ProductQuantityDiscountPrice.ToString().Replace(",", ".");

                }
                basketItems.Add(firstBasketItem);
            }

            BasketItem secondBasketItem = new BasketItem();

            secondBasketItem.Id = "BI102";
            secondBasketItem.Name = "Cargo code";
            secondBasketItem.Category1 = "Cargo";
            secondBasketItem.ItemType = BasketItemType.VIRTUAL.ToString();
            secondBasketItem.Price = cargo.Price.ToString().Replace(",", ".");
            basketItems.Add(secondBasketItem);

            request.BasketItems = basketItems;
            CheckoutFormInitialize checkoutFormInitialize = CheckoutFormInitialize.Create(request, options);


            Assert.AreEqual(Iyzipay.Model.Status.SUCCESS.ToString(), checkoutFormInitialize.Status);
            Assert.AreEqual(Locale.TR.ToString(), checkoutFormInitialize.Locale);
            Assert.IsNotNull(checkoutFormInitialize.SystemTime);
            Assert.IsNull(checkoutFormInitialize.ErrorCode);
            Assert.IsNull(checkoutFormInitialize.ErrorMessage);
            Assert.IsNull(checkoutFormInitialize.ErrorGroup);
            Assert.IsNotNull(checkoutFormInitialize.CheckoutFormContent);
            Assert.IsNotNull(checkoutFormInitialize.PaymentPageUrl);

            ViewBag.Iyzico = checkoutFormInitialize.CheckoutFormContent;

            return View();

        }


        [HttpPost]
        [Route("kontrol")]
        public ActionResult Control(RetrieveCheckoutFormRequest model)
        {
            string data = "";
            Options options = new Options();

            //options.ApiKey = "lMev3c6oqTeVtJqlrb6HED5JIK1f3Y6a";
            //options.SecretKey = "AP4f5cy6y4123Bw0Z9MGXKYf9B5li4J2";
            //options.BaseUrl = "https://api.iyzipay.com";

            options.ApiKey = "sandbox-Z92PD5nIy1dHFVLlk3vPhnjzYmC8f2MS";
            options.SecretKey = "sandbox-qrEHpoSgczPPnNJ7F0tcGGG7PcO9MEKU";
            options.BaseUrl = "https://sandbox-api.iyzipay.com";


            data = model.Token;
            RetrieveCheckoutFormRequest request = new RetrieveCheckoutFormRequest();
            request.Token = data;
            CheckoutForm checkoutForm = CheckoutForm.Retrieve(request, options);

            var adressID = Request.QueryString["addresID"];

            if (checkoutForm.PaymentStatus == "SUCCESS")
            {
                return RedirectToAction("AddOrder", new { id = adressID });
            }
            else
            {

                return Redirect("/ret/" + adressID);
            }


        }
        [HttpPost]
        public JsonResult PaymentStatus(Guid productID, int quantity)
        {
            var product = db.Products.Where(x => x.ID == productID).FirstOrDefault();
            if (Request.Cookies["Member"] == null)
            {
                string ip1 = System.Web.HttpContext.Current.Request.UserHostAddress.ToString();
                var hUser = db.HostUsers.Where(x => x.HostName == ip1).FirstOrDefault();
                if (hUser != null)
                {
                    var pBasket = db.Baskets.Where(x => x.HostUserID == hUser.ID && x.IsDelete == false && x.ProductID == productID && x.UserID == null).FirstOrDefault();

                    if (pBasket == null)
                    {

                        if (product.Stock >= quantity)
                        {
                            db.Baskets.Add(new TatliGezi.Models.Shop.Basket
                            {
                                ID = Guid.NewGuid(),
                                AddDate = DateTime.Now,
                                UpdateDate = DateTime.Now,
                                ProductID = productID,
                                Quantity = quantity,
                                HostUserID = hUser.ID,
                            });

                        }
                        else
                        {

                            ViewBag.Alert = "Stokta Ürün bulunmamaktadır.";

                        }
                    }
                    else
                    {
                        if (product.Stock >= quantity + pBasket.Quantity)
                        {
                            pBasket.Quantity += quantity;
                            ViewBag.Success = String.Format("");

                        }
                        else
                        {

                            ViewBag.Alert = "Stokta Ürün bulunmamaktadır.";


                        }
                    }
                }
                else
                {

                    HostUser user = new HostUser();
                    user.ID = Guid.NewGuid();
                    user.HostName = ip1;
                    db.HostUsers.Add(user);
                    db.SaveChanges();

                    HttpCookie myCookie = new HttpCookie("HostUser");
                    myCookie.Values["uID"] = Convert.ToString(user.ID);
                    myCookie.Expires = DateTime.Now.AddDays(1d);
                    Response.Cookies.Add(myCookie);

                    var pBasket = db.Baskets.Where(x => x.HostUserID == user.ID && x.IsDelete == false && x.ProductID == productID && x.UserID != null).FirstOrDefault();
                    if (pBasket == null)
                    {
                        if (product.Stock >= quantity)
                        {
                            db.Baskets.Add(new TatliGezi.Models.Shop.Basket
                            {
                                ID = Guid.NewGuid(),
                                AddDate = DateTime.Now,
                                UpdateDate = DateTime.Now,
                                ProductID = productID,
                                Quantity = quantity,
                                HostUserID = user.ID,
                            });
                        }
                        else
                        {
                            ViewBag.Alert = "Stokta Ürün bulunmamaktadır.";

                        }
                    }
                    else
                    {

                        if (product.Stock >= quantity + pBasket.Quantity)
                        {
                            pBasket.Quantity += quantity;

                        }
                        else
                        {
                            ViewBag.Alert = "Stokta Ürün bulunmamaktadır.";


                        }
                    }
                }


            }
            else
            {
                Guid member = new Guid(Request.Cookies["Member"].Values["uID"]);
                var pBasket = db.Baskets.Where(x => x.HostUserID == null && x.IsDelete == false && x.ProductID == productID && x.UserID != member).FirstOrDefault();
                if (pBasket == null)
                {
                    if (product.Stock >= quantity)
                    {
                        db.Baskets.Add(new TatliGezi.Models.Shop.Basket
                        {
                            ID = Guid.NewGuid(),
                            AddDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            ProductID = productID,
                            Quantity = quantity,
                            UserID = member,
                        });

                    }
                    else
                    {
                        ViewBag.Alert = "Stokta Ürün bulunmamaktadır.";


                    }
                }
                else
                {
                    if (product.Stock >= quantity + pBasket.Quantity)
                    {
                        pBasket.Quantity += quantity;

                    }
                    else
                    {
                        ViewBag.Alert = "Stokta Ürün bulunmamaktadır.";


                    }
                }
            }

            var rt = db.SaveChanges();
            return Json(new { rt, ViewBag.Alert }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult TownList(Guid? val)
        {

            var statesResults = db.Towns.Where(x => x.CityID == val).OrderBy(x => x.TownName).ToList();

            return Json(statesResults, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult CityList()
        {

            var statesResults = db.Cities.OrderBy(x => x.CityName).ToList();

            return Json(statesResults, JsonRequestBehavior.AllowGet);

        }
    }
}