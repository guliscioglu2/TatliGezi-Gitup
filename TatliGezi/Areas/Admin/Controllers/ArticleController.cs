using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TatliGezi.Models;
using TatliGezi.Models.MultipleData;
using System.Threading.Tasks;
//using MongoDB.Bson.Serialization.IdGenerators;

namespace TatliGezi.Areas.Admin.Controllers
{
    public class ArticleController : BaseAdminController
    {
        // GET: Admin/Article
        void drpDoldur()
        {
            List<ArticleCategory> cList = db.ArticleCategories.Where(x => x.IsDelete == false).ToList();
            List<SelectListItem> sList = cList.Select(x => new SelectListItem()
            {
                Text = x.CategoryName,
                Value = x.ID.ToString()
            }).ToList();
            ViewData["Category"] = sList;
        }
        void drpDoldurEdit(Guid? id)
        {
            List<ArticleCategory> cList = db.ArticleCategories.Where(x => x.IsDelete == false).ToList();
            List<SelectListItem> sList = cList.Select(x => new SelectListItem()

            {
                Text = x.CategoryName,
                Value = x.ID.ToString()


            }).ToList();
            foreach (var item in sList)
            {
                if (item.Value == id.ToString())
                {
                    item.Selected = true;
                }


            }
            ViewData["Category"] = sList;
        }


        public ActionResult Index()
        {

            if (Request.Cookies["Admin"] != null)
            {
                List<Article> aList = db.Articles.Where(x => x.IsDelete == false && x.IsActive == true).OrderByDescending(x => x.AddDate).ToList();


                return View(aList);
            }
            else
            {
                return Redirect("/Admin/LoginAdmin/Index/");
            }
        }
        public ActionResult Taslak()
        {

            if (Request.Cookies["Admin"] != null)
            {
                List<Article> aList = db.Articles.Where(x => x.IsDelete == false && x.IsActive == false).OrderByDescending(x => x.AddDate).ToList();

                return View(aList);
            }
            else
            {
                return Redirect("/Admin/LoginAdmin/Index/");
            }
        }

        public ActionResult ArticleFormat()
        {

            if (Request.Cookies["Admin"] != null)
            {

                drpDoldur();
                ArticleAdd _model = new ArticleAdd();
                _model.fList = db.Formats.ToList();
                return View(_model);
            }
            else
            {
                return Redirect("/Admin/LoginAdmin/Index/");
            }

        }
        public ActionResult ArticleAdd(Guid id)
        {
            if (Request.Cookies["Admin"] != null)
            {
                ArticleAdd _model = new ArticleAdd();
                var format = db.Formats.Find(id);
                _model.value = format.Value;
                Session["FormatID"] = id;
                Session["FormatValue"] = format.Value;

                drpDoldur();

                return View(_model);
            }
            else
            {
                return Redirect("/Admin/LoginAdmin/Index/");
            }


        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ArticleAdd(Article article, Preview preview, string[] CategoryList, HttpPostedFileBase resim, HttpPostedFileBase resim2, HttpPostedFileBase resim3, HttpPostedFileBase music, string command)
        {

            if (command.Equals("Yayınla"))
            {
                drpDoldur();



                //if (article.ArticleTitle != null && article.ArticleContent != null )
                //{
                    var formatID = Session["FormatID"].ToString();

                    article.ID = Guid.NewGuid();
                    article.FormatID = Guid.Parse(formatID);
                    article.AddDate = DateTime.Now;
                    article.UpdateDate = DateTime.Now;
                    article.IsDelete = false;
                    article.IsActive = true;

                    if (resim != null)
                    {
                        var fileName = article.ArticleTitle + resim.FileName;
                        var path = Server.MapPath("/Images/Blog/" + fileName);
                        resim.SaveAs(path);
                        article.PhotoPath1 = "/Images/Blog/" + fileName;
                    }

                    if (resim2 != null)
                    {
                        var fileName = article.ArticleTitle + resim2.FileName;
                        var path = Server.MapPath("/Images/Blog/" + fileName);
                        resim2.SaveAs(path);
                        article.PhotoPath2 = "/Images/Blog/" + fileName;
                    }

                    if (resim3 != null)
                    {
                        var fileName = article.ArticleTitle + resim3.FileName;
                        var path = Server.MapPath("/Images/Blog/" + fileName);
                        resim3.SaveAs(path);
                        article.PhotoPath3 = "/Images/Blog/" + fileName;
                    }


                    if (music != null)
                    {
                        var fileName = article.ArticleTitle + music.FileName;
                        var path = Server.MapPath("/Musics/" + fileName);
                        music.SaveAs(path);
                        article.Audio = "/Musics/" + fileName;
                    }
                    foreach (var item in CategoryList)
                    {
                        article.ArticleCategoryID = Guid.Parse(item);
                    }

                    db.Articles.Add(article);

                
                    db.SaveChanges();

                 
                    //foreach (var item in CategoryList)
                    //{
                    //    ArticleCategoryJoin ac = new ArticleCategoryJoin();
                    //    ac.ArticleID = article.ID;
                    //    ac.ArticleCategoryID = Guid.Parse(item);
                    //    db.ArticleCategoryJoins.Add(ac);
                    //    db.SaveChanges();
                    //}


                //}


                //else
                //{
                //    ViewBag.Durum = -1;
                //    return View();
                //}

            }


            if (command.Equals("Taslak Olarak Kaydet"))
            {
                drpDoldur();

                if ((int)Session["FormatValue"] == 1)
                {
                    if (resim != null)
                    {
                        var fileName = DateTime.Now.ToString("yyMMddHHmmssff") + resim.FileName;
                        var path = Server.MapPath("/Images/Blog/" + fileName);
                        resim.SaveAs(path);
                        article.PhotoPath1 = "/Images/Blog/" + fileName;
                    }

                    if (article.ArticleTitle != null && article.ArticleContent != null & article.PhotoPath1 != null)
                    {
                        article.ID = Guid.NewGuid();

                        article.Format.Value = 1;
                        article.AddDate = DateTime.Now;
                        article.UpdateDate = DateTime.Now;
                        article.IsDelete = false;
                        article.IsActive = false;

                        db.Articles.Add(article);
                        db.SaveChanges();

                        foreach (var item in CategoryList)
                        {
                            article.ArticleCategoryID = new Guid(item);
                        }
                        foreach (var item in CategoryList)
                        {
                            ArticleCategoryJoin ac = new ArticleCategoryJoin();
                            ac.ID = Guid.NewGuid();
                            ac.ArticleID = article.ID;
                            ac.ArticleCategoryID = new Guid(item);
                            db.ArticleCategoryJoins.Add(ac);
                            db.SaveChanges();
                        }


                    }
                    return RedirectToAction("Index");
                }
                if ((int)Session["FormatValue"] == 2)
                {
                    if (resim != null)
                    {
                        var fileName = DateTime.Now.ToString("yyMMddHHmmssff") + resim.FileName;
                        var path = Server.MapPath("/Images/Blog/" + fileName);
                        resim.SaveAs(path);
                        article.PhotoPath1 = "/Images/Blog/" + fileName;
                    }
                    if (music != null)
                    {
                        var fileName = DateTime.Now.ToString("yyMMddHHmmssff") + music.FileName;
                        var path = Server.MapPath("/Musics/" + fileName);
                        music.SaveAs(path);
                        article.Audio = "/Musics/" + fileName;
                    }
                    if (article.ArticleTitle != null && article.ArticleContent != null & article.PhotoPath1 != null & article.Audio != null)
                    {
                        article.ID = Guid.NewGuid();

                        article.Format.Value = 2;


                        article.AddDate = DateTime.Now;
                        article.UpdateDate = DateTime.Now;
                        article.IsDelete = false;
                        article.IsActive = false;

                        db.Articles.Add(article);
                        db.SaveChanges();

                        foreach (var item in CategoryList)
                        {
                            article.ArticleCategoryID = new Guid(item);
                        }
                        foreach (var item in CategoryList)
                        {
                            ArticleCategoryJoin ac = new ArticleCategoryJoin();
                            ac.ID = Guid.NewGuid();
                            ac.ArticleID = article.ID;
                            ac.ArticleCategoryID = new Guid(item);
                            db.ArticleCategoryJoins.Add(ac);
                            db.SaveChanges();
                        }

                    }
                    return RedirectToAction("Index");

                }
                if ((int)Session["FormatValue"] == 3)
                {
                    if (resim != null)
                    {
                        var fileName = DateTime.Now.ToString("yyMMddHHmmssff") + resim.FileName;
                        var path = Server.MapPath("/Images/Blog/" + fileName);
                        resim.SaveAs(path);
                        article.PhotoPath1 = "/Images/Blog/" + fileName;
                    }
                    if (article.ArticleTitle != null && article.ArticleContent != null & article.PhotoPath1 != null & article.Url != null)
                    {
                        article.ID = Guid.NewGuid();

                        article.Format.Value = 3;

                        article.AddDate = DateTime.Now;
                        article.UpdateDate = DateTime.Now;
                        article.IsDelete = false;
                        article.IsActive = false;

                        db.Articles.Add(article);
                        db.SaveChanges();

                        foreach (var item in CategoryList)
                        {
                            article.ArticleCategoryID = new Guid(item);
                        }
                        foreach (var item in CategoryList)
                        {
                            ArticleCategoryJoin ac = new ArticleCategoryJoin();
                            ac.ID = Guid.NewGuid();
                            ac.ArticleID = article.ID;
                            ac.ArticleCategoryID = new Guid(item);
                            db.ArticleCategoryJoins.Add(ac);
                            db.SaveChanges();
                        }

                    }
                    return RedirectToAction("Index");

                }
                if ((int)Session["FormatValue"] == 4)
                {
                    if (resim != null)
                    {
                        var fileName = DateTime.Now.ToString("yyMMddHHmmssff") + resim.FileName;
                        var path = Server.MapPath("/Images/Blog/" + fileName);
                        resim.SaveAs(path);
                        article.PhotoPath1 = "/Images/Blog/" + fileName;
                    }
                    if (resim2 != null)
                    {
                        var fileName = DateTime.Now.ToString("yyMMddHHmmssff") + resim2.FileName;
                        var path = Server.MapPath("/Images/Blog/" + fileName);
                        resim2.SaveAs(path);
                        article.PhotoPath2 = "/Images/Blog/" + fileName;
                    }
                    if (resim3 != null)
                    {
                        var fileName = DateTime.Now.ToString("yyMMddHHmmssff") + resim3.FileName;
                        var path = Server.MapPath("/Images/Blog/" + fileName);
                        resim3.SaveAs(path);
                        article.PhotoPath3 = "/Images/Blog/" + fileName;
                    }

                    if (article.ArticleTitle != null && article.ArticleContent != null & article.PhotoPath1 != null & article.PhotoPath2 != null & article.PhotoPath3 != null)
                    {

                        article.ID = Guid.NewGuid();

                        article.Format.Value = 4;

                        article.AddDate = DateTime.Now;
                        article.UpdateDate = DateTime.Now;
                        article.IsDelete = false;
                        article.IsActive = false;

                        db.Articles.Add(article);
                        db.SaveChanges();

                        foreach (var item in CategoryList)
                        {
                            article.ArticleCategoryID = new Guid(item);
                        }
                        foreach (var item in CategoryList)
                        {
                            ArticleCategoryJoin ac = new ArticleCategoryJoin();
                            ac.ID = Guid.NewGuid();
                            ac.ArticleID = article.ID;
                            ac.ArticleCategoryID = new Guid(item);
                            db.ArticleCategoryJoins.Add(ac);
                            db.SaveChanges();
                        }

                    }
                    return RedirectToAction("Index");

                }

                if ((int)Session["FormatValue"] == 5)
                {
                    if (resim != null)
                    {
                        var fileName = DateTime.Now.ToString("yyMMddHHmmssff") + resim.FileName;
                        var path = Server.MapPath("/Images/Blog/" + fileName);
                        resim.SaveAs(path);
                        article.PhotoPath1 = "/Images/Blog/" + fileName;
                    }
                    if (article.ArticleTitle != null && article.ArticleContent != null & article.PhotoPath1 != null)
                    {
                        article.ID = Guid.NewGuid();

                        article.Format.Value = 5;
                        article.AddDate = DateTime.Now;
                        article.UpdateDate = DateTime.Now;
                        article.IsDelete = false;
                        article.IsActive = false;

                        db.Articles.Add(article);
                        db.SaveChanges();

                        foreach (var item in CategoryList)
                        {
                            article.ArticleCategoryID = new Guid(item);
                        }
                        foreach (var item in CategoryList)
                        {
                            ArticleCategoryJoin ac = new ArticleCategoryJoin();
                            ac.ID = Guid.NewGuid();
                            ac.ArticleID = article.ID;
                            ac.ArticleCategoryID = new Guid(item);
                            db.ArticleCategoryJoins.Add(ac);
                            db.SaveChanges();
                        }

                    }
                    return RedirectToAction("Index");

                }

                else
                {
                    ViewBag.Durum = -1;
                    return View();
                }

            }

            if(command.Equals("Önizleme"))

            {
                drpDoldur();


                if ((int)Session["FormatID"] == 1)
                {
                    if (resim != null)
                    {
                        var fileName = DateTime.Now.ToString("yyMMddHHmmssff") + resim.FileName;
                        var path = Server.MapPath("/Images/Blog/" + fileName);
                        resim.SaveAs(path);
                        preview.PhotoPath1 = "/Images/Blog/" + fileName;
                    }
                    //if (preview.PhotoPath1 == null)
                    //{
                    //    preview.PhotoPath1 = Session["Photo"].ToString();
                    //}

                    if (article.ArticleTitle != null && article.ArticleContent != null)
                    {

                        preview.Format.Value = 1;

                        preview.AddDate = DateTime.Now;
                        preview.UpdateDate = DateTime.Now;
                        preview.IsDelete = false;
                        preview.ArticleTitle = article.ArticleTitle;
                        preview.ArticleContent = article.ArticleContent;
                        db.Previews.Add(preview);
                        db.SaveChanges();
                        Session["Preview"] = preview.ID;

                        foreach (var item in CategoryList)
                        {
                            preview.ArticleCategoryID = new Guid(item);
                            db.SaveChanges();
                        }

                        return RedirectToAction("Detay1", new { id = Session["Preview"] });

                    }
                    else
                    {
                        ViewBag.Durum = -1;
                        return View();
                    }
                }

                if ((int)Session["FormatID"] == 2)
                {

                    if (resim != null)
                    {
                        var fileName = DateTime.Now.ToString("yyMMddHHmmssff") + resim.FileName;
                        var path = Server.MapPath("/Images/Blog/" + fileName);
                        resim.SaveAs(path);
                        preview.PhotoPath1 = "/Images/Blog/" + fileName;
                    }

                    if (music != null)
                    {
                        var fileName = DateTime.Now.ToString("yyMMddHHmmssff") + music.FileName;
                        var path = Server.MapPath("/Music/" + fileName);
                        music.SaveAs(path);
                        preview.Audio = "/Music/" + fileName;
                    }

                    //if (preview.PhotoPath1 == null)
                    //{
                    //    preview.PhotoPath1 = Session["Photo"].ToString();
                    //}

                    if (article.ArticleTitle != null && article.ArticleContent != null)
                    {

                        preview.Format.Value = 2;

                        preview.AddDate = DateTime.Now;
                        preview.UpdateDate = DateTime.Now;
                        preview.IsDelete = false;
                        preview.ArticleTitle = article.ArticleTitle;
                        preview.ArticleContent = article.ArticleContent;
                        db.Previews.Add(preview);
                        db.SaveChanges();
                        Session["Preview"] = preview.ID;

                        foreach (var item in CategoryList)
                        {
                            preview.ArticleCategoryID = new Guid(item);
                            db.SaveChanges();
                        }

                        return RedirectToAction("Detay1", new { id = Session["Preview"] });

                    }
                    else
                    {
                        ViewBag.Durum = -1;
                        return View();
                    }








                }


                if ((int)Session["FormatID"] == 3)
                {

                    if (resim != null)
                    {
                        var fileName = DateTime.Now.ToString("yyMMddHHmmssff") + resim.FileName;
                        var path = Server.MapPath("/Images/Blog/" + fileName);
                        resim.SaveAs(path);
                        preview.PhotoPath1 = "/Images/Blog/" + fileName;
                    }
                    if (preview.PhotoPath1 == null)
                    {
                        preview.PhotoPath1 = Session["Photo"].ToString();
                    }

                    if (article.ArticleTitle != null && article.ArticleContent != null)
                    {

                        preview.Format.Value = 3;
                        preview.AddDate = DateTime.Now;
                        preview.UpdateDate = DateTime.Now;
                        preview.IsDelete = false;
                        preview.Url = article.Url;

                        preview.ArticleTitle = article.ArticleTitle;
                        preview.ArticleContent = article.ArticleContent;
                        db.Previews.Add(preview);
                        db.SaveChanges();
                        Session["Preview"] = preview.ID;

                        foreach (var item in CategoryList)
                        {
                            preview.ArticleCategoryID = new Guid(item);
                            db.SaveChanges();
                        }

                        return RedirectToAction("Detay1", new { id = Session["Preview"] });

                    }
                    else
                    {
                        ViewBag.Durum = -1;
                        return View();
                    }

                }
                if ((int)Session["FormatID"] == 4)
                {

                    if (resim != null)
                    {
                        var fileName = DateTime.Now.ToString("yyMMddHHmmssff") + resim.FileName;
                        var path = Server.MapPath("/Images/Blog/" + fileName);
                        resim.SaveAs(path);
                        preview.PhotoPath1 = "/Images/Blog/" + fileName;
                    }
                    if (preview.PhotoPath1 == null)
                    {
                        preview.PhotoPath1 = Session["Photo"].ToString();
                    }
                    if (resim2 != null)
                    {
                        var fileName = DateTime.Now.ToString("yyMMddHHmmssff") + resim2.FileName;
                        var path = Server.MapPath("/Images/Blog/" + fileName);
                        resim2.SaveAs(path);
                        preview.PhotoPath2 = "/Images/Blog/" + fileName;
                    }
                    if (preview.PhotoPath2 == null)
                    {
                        preview.PhotoPath2 = Session["Photo"].ToString();
                    }
                    if (resim3 != null)
                    {
                        var fileName = DateTime.Now.ToString("yyMMddHHmmssff") + resim3.FileName;
                        var path = Server.MapPath("/Images/Blog/" + fileName);
                        resim3.SaveAs(path);
                        preview.PhotoPath3 = "/Images/Blog/" + fileName;
                    }
                    if (preview.PhotoPath3 == null)
                    {
                        preview.PhotoPath3 = Session["Photo"].ToString();
                    }

                    if (article.ArticleTitle != null && article.ArticleContent != null)
                    {

                        preview.Format.Value = 4;
                        preview.AddDate = DateTime.Now;
                        preview.UpdateDate = DateTime.Now;
                        preview.IsDelete = false;
                        preview.ArticleTitle = article.ArticleTitle;
                        preview.ArticleContent = article.ArticleContent;
                        db.Previews.Add(preview);
                        db.SaveChanges();
                        Session["Preview"] = preview.ID;

                        foreach (var item in CategoryList)
                        {
                            preview.ArticleCategoryID = new Guid(item);
                            db.SaveChanges();
                        }

                        return RedirectToAction("Detay1", new { id = Session["Preview"] });

                    }
                    else
                    {
                        ViewBag.Durum = -1;
                        return View();
                    }

                }

                if ((int)Session["FormatID"] == 5)
                {

                    if (resim != null)
                    {
                        var fileName = DateTime.Now.ToString("yyMMddHHmmssff") + resim.FileName;
                        var path = Server.MapPath("/Images/Blog/" + fileName);
                        resim.SaveAs(path);
                        preview.PhotoPath1 = "/Images/Blog/" + fileName;
                    }
                    if (preview.PhotoPath1 == null)
                    {
                        preview.PhotoPath1 = Session["Photo"].ToString();
                    }

                    if (article.ArticleTitle != null && article.ArticleContent != null)
                    {

                        preview.Format.Value = 5;
                        preview.AddDate = DateTime.Now;
                        preview.UpdateDate = DateTime.Now;
                        preview.IsDelete = false;
                        preview.ArticleTitle = article.ArticleTitle;
                        preview.Author = article.Author;

                        preview.QuoteContent = article.QuoteContent;

                        preview.ArticleContent = article.ArticleContent;
                        db.Previews.Add(preview);
                        db.SaveChanges();
                        Session["Preview"] = preview.ID;

                        foreach (var item in CategoryList)
                        {
                            preview.ArticleCategoryID = new Guid(item);
                            db.SaveChanges();
                        }

                        return RedirectToAction("Detay1", new { id = Session["Preview"] });

                    }
                    else
                    {
                        ViewBag.Durum = -1;
                        return View();
                    }

                }
                else
                {
                    ViewBag.Durum = -1;
                    return View();
                }



            }

            return RedirectToAction("Index");

        }



        public ActionResult ArticleEdit(Guid id)
        {
            if (Request.Cookies["Admin"] != null)
            {
                ArticleAdd _model = new ArticleAdd();
                _model.article = db.Articles.Find(id);
                _model.value = _model.article.Format.Value;
                Session["FormatID"] = _model.article.Format.Value;
                drpDoldur();

                drpDoldurEdit(_model.article.ArticleCategoryID);

                Session["Article"] = id;
                Session["Photo"] = _model.article.PhotoPath1;
                Session["Photo2"] = _model.article.PhotoPath2;

                Session["Photo3"] = _model.article.PhotoPath3;


                return View(_model);
            }
            else
            {
                return Redirect("/Admin/LoginAdmin/Index/");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ArticleEdit(Preview preview, Article article, string[] CategoryList, HttpPostedFileBase music, HttpPostedFileBase resim, HttpPostedFileBase resim2, HttpPostedFileBase resim3, string command)
        {

            if (command.Equals("Kaydet"))
            {
                drpDoldur();

       
                if (article.ArticleTitle != null && article.ArticleContent != null)
                {

                    Article dbArticle = db.Articles.Find(article.ID);
                    if (resim != null)
                    {
                        var fileName = DateTime.Now.ToString("yyMMddHHmmssff") + resim.FileName;
                        var path = Server.MapPath("/Images/Blog/" + fileName);
                        resim.SaveAs(path);
                        dbArticle.PhotoPath1 = "/Images/Blog/" + fileName;
                    }
                    else
                    {
                        article.PhotoPath1 = dbArticle.PhotoPath1;
                    }
                    if (resim2 != null)
                    {
                        var fileName = DateTime.Now.ToString("yyMMddHHmmssff") + resim2.FileName;
                        var path = Server.MapPath("/Images/Blog/" + fileName);
                        resim2.SaveAs(path);
                        dbArticle.PhotoPath2 = "/Images/Blog/" + fileName;
                    }
                    else
                    {
                        article.PhotoPath2 = dbArticle.PhotoPath2;

                    }
                    if (resim3 != null)
                    {
                        var fileName = DateTime.Now.ToString("yyMMddHHmmssff") + resim3.FileName;
                        var path = Server.MapPath("/Images/Blog/" + fileName);
                        resim3.SaveAs(path);
                        dbArticle.PhotoPath3 = "/Images/Blog/" + fileName;
                    }
                    else
                    {
                        article.PhotoPath3 = dbArticle.PhotoPath3;

                    }
                    if (music != null)
                    {
                        var fileName = DateTime.Now.ToString("yyMMddHHmmssff") + music.FileName;
                        var path = Server.MapPath("/Musics/" + fileName);
                        music.SaveAs(path);
                        article.Audio = "/Musics/" + fileName;
                    }
                    else
                    {
                        article.Audio = dbArticle.Audio;
                    }

                    article.AddDate = dbArticle.AddDate;
                    dbArticle.UpdateDate = DateTime.Now;
                    dbArticle.ArticleTitle = article.ArticleTitle;
                    dbArticle.ArticleContent = article.ArticleContent;
                    dbArticle.UpdateDate = DateTime.Now;
                    dbArticle.Url = article.Url;
                    dbArticle.Author = article.Author;
                    dbArticle.QuoteContent = article.QuoteContent;

                    //if (article.PhotoPath1 != null)
                    //{
                    //    dbArticle.PhotoPath1 = article.PhotoPath1;
                    //}
                    foreach (var item in CategoryList)
                    {
                        if (item != dbArticle.ArticleCategoryID.ToString())
                            dbArticle.ArticleCategoryID = Guid.Parse(item);
                    }

                    db.SaveChanges();

                    


                    //List<ArticleCategoryJoin> alist = db.ArticleCategoryJoins.Where(x => x.ArticleID == article.ID).ToList();
                    //foreach (var item in alist)
                    //{
                    //    db.ArticleCategoryJoins.Remove(item);
                    //    db.SaveChanges();
                    //}

                    //foreach (var item in CategoryList)
                    //{
                    //    ArticleCategoryJoin ac = new ArticleCategoryJoin();
                    //    ac.ID = Guid.NewGuid();
                    //    ac.ArticleID = article.ID;
                    //    ac.ArticleCategoryID = new Guid(item);
                    //    db.ArticleCategoryJoins.Add(ac);
                    //    db.SaveChanges();
                    //}


                }

                return RedirectToAction("Index");













            }
            else
            {
                drpDoldur();


                if ((int)Session["FormatID"] == 1)
                {
                    if (resim != null)
                    {
                        var fileName = DateTime.Now.ToString("yyMMddHHmmssff") + resim.FileName;
                        var path = Server.MapPath("/Images/Blog/" + fileName);
                        resim.SaveAs(path);
                        preview.PhotoPath1 = "/Images/Blog/" + fileName;
                    }
                    if (preview.PhotoPath1 == null)
                    {
                        preview.PhotoPath1 = Session["Photo"].ToString(); ;
                    }

                    if (article.ArticleTitle != null && article.ArticleContent != null)
                    {

                        preview.FormatID = 1;
                        preview.AddDate = DateTime.Now;
                        preview.UpdateDate = DateTime.Now;
                        preview.IsDelete = false;
                        preview.ArticleTitle = article.ArticleTitle;
                        preview.ArticleContent = article.ArticleContent;
                        db.Previews.Add(preview);
                        db.SaveChanges();
                        Session["Preview"] = preview.ID;

                        foreach (var item in CategoryList)
                        {
                            preview.ArticleCategoryID = new Guid(item);
                            db.SaveChanges();
                        }

                        return RedirectToAction("Detay", new { id = Session["Preview"] });

                    }
                    else
                    {
                        ViewBag.Durum = -1;
                        return View();
                    }
                }

                if ((int)Session["FormatID"] == 2)
                {

                    if (resim != null)
                    {
                        var fileName = DateTime.Now.ToString("yyMMddHHmmssff") + resim.FileName;
                        var path = Server.MapPath("/Images/Blog/" + fileName);
                        resim.SaveAs(path);
                        preview.PhotoPath1 = "/Images/Blog/" + fileName;
                    }

                    if (music != null)
                    {
                        var fileName = DateTime.Now.ToString("yyMMddHHmmssff") + music.FileName;
                        var path = Server.MapPath("/Music/" + fileName);
                        music.SaveAs(path);
                        preview.Audio = "/Music/" + fileName;
                    }

                    if (preview.PhotoPath1 == null)
                    {
                        preview.PhotoPath1 = Session["Photo"].ToString();
                    }

                    if (article.ArticleTitle != null && article.ArticleContent != null)
                    {

                        preview.FormatID = 2;

                        preview.AddDate = DateTime.Now;
                        preview.UpdateDate = DateTime.Now;
                        preview.IsDelete = false;
                        preview.ArticleTitle = article.ArticleTitle;
                        preview.ArticleContent = article.ArticleContent;
                        db.Previews.Add(preview);
                        db.SaveChanges();
                        Session["Preview"] = preview.ID;

                        foreach (var item in CategoryList)
                        {
                            preview.ArticleCategoryID = new Guid(item);
                            db.SaveChanges();
                        }

                        return RedirectToAction("Detay", new { id = Session["Preview"] });

                    }
                    else
                    {
                        ViewBag.Durum = -1;
                        return View();
                    }








                }


                if ((int)Session["FormatID"] == 3)
                {

                    if (resim != null)
                    {
                        var fileName = DateTime.Now.ToString("yyMMddHHmmssff") + resim.FileName;
                        var path = Server.MapPath("/Images/Blog/" + fileName);
                        resim.SaveAs(path);
                        preview.PhotoPath1 = "/Images/Blog/" + fileName;
                    }
                    if (preview.PhotoPath1 == null)
                    {
                        preview.PhotoPath1 = Session["Photo"].ToString();
                    }

                    if (article.ArticleTitle != null && article.ArticleContent != null)
                    {

                        preview.FormatID = 3;
                        preview.AddDate = DateTime.Now;
                        preview.UpdateDate = DateTime.Now;
                        preview.IsDelete = false;
                        preview.Url = article.Url;

                        preview.ArticleTitle = article.ArticleTitle;
                        preview.ArticleContent = article.ArticleContent;
                        db.Previews.Add(preview);
                        db.SaveChanges();
                        Session["Preview"] = preview.ID;

                        foreach (var item in CategoryList)
                        {
                            preview.ArticleCategoryID = new Guid(item);
                            db.SaveChanges();
                        }

                        return RedirectToAction("Detay", new { id = Session["Preview"] });

                    }
                    else
                    {
                        ViewBag.Durum = -1;
                        return View();
                    }

                }
                if ((int)Session["FormatID"] == 4)
                {

                    if (resim != null)
                    {
                        var fileName = DateTime.Now.ToString("yyMMddHHmmssff") + resim.FileName;
                        var path = Server.MapPath("/Images/Blog/" + fileName);
                        resim.SaveAs(path);
                        preview.PhotoPath1 = "/Images/Blog/" + fileName;
                    }
                    if (preview.PhotoPath1 == null)
                    {
                        preview.PhotoPath1 = Session["Photo"].ToString();
                    }
                    if (resim2 != null)
                    {
                        var fileName = DateTime.Now.ToString("yyMMddHHmmssff") + resim2.FileName;
                        var path = Server.MapPath("/Images/Blog/" + fileName);
                        resim2.SaveAs(path);
                        preview.PhotoPath2 = "/Images/Blog/" + fileName;
                    }
                    if (preview.PhotoPath2 == null)
                    {
                        preview.PhotoPath2 = Session["Photo2"].ToString();
                    }
                    if (resim3 != null)
                    {
                        var fileName = DateTime.Now.ToString("yyMMddHHmmssff") + resim3.FileName;
                        var path = Server.MapPath("/Images/Blog/" + fileName);
                        resim3.SaveAs(path);
                        preview.PhotoPath3 = "/Images/Blog/" + fileName;
                    }
                    if (preview.PhotoPath3 == null)
                    {
                        preview.PhotoPath3 = Session["Photo3"].ToString();
                    }

                    if (article.ArticleTitle != null && article.ArticleContent != null)
                    {

                        preview.FormatID = 4;
                        preview.AddDate = DateTime.Now;
                        preview.UpdateDate = DateTime.Now;
                        preview.IsDelete = false;
                        preview.ArticleTitle = article.ArticleTitle;
                        preview.ArticleContent = article.ArticleContent;
                        db.Previews.Add(preview);
                        db.SaveChanges();
                        Session["Preview"] = preview.ID;

                        foreach (var item in CategoryList)
                        {
                            preview.ArticleCategoryID = new Guid(item);
                            db.SaveChanges();
                        }

                        return RedirectToAction("Detay", new { id = Session["Preview"] });

                    }
                    else
                    {
                        ViewBag.Durum = -1;
                        return View();
                    }

                }

                if ((int)Session["FormatID"] == 5)
                {

                    if (resim != null)
                    {
                        var fileName = DateTime.Now.ToString("yyMMddHHmmssff") + resim.FileName;
                        var path = Server.MapPath("/Images/Blog/" + fileName);
                        resim.SaveAs(path);
                        preview.PhotoPath1 = "/Images/Blog/" + fileName;
                    }
                    if (preview.PhotoPath1 == null)
                    {
                        preview.PhotoPath1 = Session["Photo"].ToString();
                    }

                    if (article.ArticleTitle != null && article.ArticleContent != null)
                    {

                        preview.FormatID = 5;
                        preview.AddDate = DateTime.Now;
                        preview.UpdateDate = DateTime.Now;
                        preview.IsDelete = false;
                        preview.ArticleTitle = article.ArticleTitle;
                        preview.Author = article.Author;

                        preview.QuoteContent = article.QuoteContent;

                        preview.ArticleContent = article.ArticleContent;
                        db.Previews.Add(preview);
                        db.SaveChanges();
                        Session["Preview"] = preview.ID;

                        foreach (var item in CategoryList)
                        {
                            preview.ArticleCategoryID = new Guid(item);
                            db.SaveChanges();
                        }

                        return RedirectToAction("Detay", new { id = Session["Preview"] });

                    }
                    else
                    {
                        ViewBag.Durum = -1;
                        return View();
                    }

                }
                else
                {
                    ViewBag.Durum = -1;
                    return View();
                }



            }




        }

        public ActionResult TagAdd(Guid id)
        {
            //Article art = db.Articles.Find(id);
            //return View(art);
            Session["ArtID"] = id;

            return View();
        }
        [HttpPost]
        public ActionResult Tags(ArticleTag tag, List<string> items)
        {
            string _result = "";
            string elements;
            elements = items.ToString();
            for (int i = 0; i < items.Count(); i++)
            {
                string[] _split = items[i].Split('½');

                string _Detail = _split[0];

                var articleID = Session["ArtID"].ToString();
                tag.ArticleID = Guid.Parse(articleID);
                tag.AddDate = DateTime.Now;
                tag.UpdateDate = DateTime.Now;
                tag.IsDelete = false;
                tag.TagName = _Detail;
                db.ArticleTags.Add(tag);
                db.SaveChanges();


            }
            return Json(JsonRequestBehavior.AllowGet);

            //return RedirectToAction("Index");
        }
        //[HttpPost]
        //public ActionResult TagAdd(string TagName, Tag tag)
        //{

        //    tag.TagName = TagName;

        //    //comment.UserID = Convert.ToInt32(Session["UserID"]);
        //    //comment.ArticleID = Convert.ToInt32(Session["ArticleID"]);
        //    tag.AddDate = DateTime.Now;
        //    tag.IsDelete = false;
        //    tag.UpdateDate = DateTime.Now;
        //    db.Tags.Add(tag);
        //    db.SaveChanges();
        //    string z = TagName;
        //    return Json(z, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult PrevSave(Guid id)
        {
            if (Request.Cookies["Admin"] != null)
            {
                ArticlePreview _model = new ArticlePreview();
                _model.preview = db.Previews.Find(id);
                Session["Photo"] = _model.preview.PhotoPath1;
                Session["Photo2"] = _model.preview.PhotoPath2;

                Session["Photo3"] = _model.preview.PhotoPath3;
                Session["FormatID"] = _model.preview.FormatID;

                drpDoldur();
                drpDoldurEdit(_model.preview.ArticleCategoryID);



                return View(_model);
            }
            else
            {
                return Redirect("/Admin/LoginAdmin/Index/");
            }
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PrevSave(int id, Preview preview, Article article, string[] CategoryList, HttpPostedFileBase resim, HttpPostedFileBase resim2, HttpPostedFileBase resim3, HttpPostedFileBase music, string command)
        {
            if (command.Equals("Kaydet"))
            {
                drpDoldur();

                if ((int)Session["FormatID"] == 1)
                {
                    if (resim != null)
                    {
                        var filename = DateTime.Now.ToString("yyMMddHHmmssff") + resim.FileName;
                        var path = Server.MapPath("/Images/Blog/" + filename);
                        resim.SaveAs(path);
                        preview.PhotoPath1 = "/Images/Blog/" + filename;
                    }




                    Guid idArt = (Guid)Session["Article"];
                    Article dbArt = db.Articles.Find(idArt);
                    article.AddDate = dbArt.AddDate;
                    dbArt.UpdateDate = DateTime.Now;

                    Preview dbPre = db.Previews.Find(preview.ID);
                    dbPre.ArticleTitle = preview.ArticleTitle;
                    dbPre.ArticleContent = preview.ArticleContent;


                    foreach (var item in CategoryList)
                    {
                        if (item != dbPre.ArticleCategoryID.ToString())
                            dbPre.ArticleCategoryID = new Guid(item);
                    }

                    if (preview.PhotoPath1 == null)
                    {
                        preview.PhotoPath1 = dbPre.PhotoPath1;
                    }
                    preview.ArticleCategoryID = dbPre.ArticleCategoryID;
                    db.SaveChanges();

                    dbArt.ArticleTitle = preview.ArticleTitle;
                    dbArt.ArticleContent = preview.ArticleContent;
                    dbArt.PhotoPath1 = preview.PhotoPath1;
                    dbArt.ArticleCategoryID = preview.ArticleCategoryID;
                    db.SaveChanges();
                    db.Previews.Remove(dbPre);
                    db.SaveChanges();

                    return RedirectToAction("Index");

                }

                if ((int)Session["FormatID"] == 2)
                {
                    if (resim != null)
                    {
                        drpDoldur();
                        var filename = DateTime.Now.ToString("yyMMddHHmmssff") + resim.FileName;
                        var path = Server.MapPath("/Images/Blog/" + filename);
                        resim.SaveAs(path);
                        article.PhotoPath1 = "/Images/Blog/" + filename;
                    }

                    if (music != null)
                    {
                        var fileName = DateTime.Now.ToString("yyMMddHHmmssff") + music.FileName;
                        var path = Server.MapPath("/Music/" + fileName);
                        music.SaveAs(path);
                        article.Audio = "/Music/" + fileName;
                    }


                    Guid idArt = (Guid)Session["Article"];
                    Article dbArt = db.Articles.Find(idArt);
                    article.AddDate = dbArt.AddDate;
                    dbArt.UpdateDate = DateTime.Now;

                    Preview dbPre = db.Previews.Find(preview.ID);
                    dbPre.ArticleTitle = preview.ArticleTitle;
                    dbPre.ArticleContent = preview.ArticleContent;


                    foreach (var item in CategoryList)
                    {
                        if (item != dbPre.ArticleCategoryID.ToString())
                            dbPre.ArticleCategoryID = new Guid(item);
                    }

                    if (article.PhotoPath1 == null)
                    {
                        article.PhotoPath1 = dbPre.PhotoPath1;
                    }
                    preview.ArticleCategoryID = dbPre.ArticleCategoryID;
                    db.SaveChanges();

                    dbArt.ArticleTitle = preview.ArticleTitle;
                    dbArt.ArticleContent = preview.ArticleContent;
                    dbArt.ArticleCategoryID = preview.ArticleCategoryID;
                    db.SaveChanges();
                    drpDoldur();
                    db.Previews.Remove(dbPre);
                    db.SaveChanges();

                    return RedirectToAction("Index");

                }


                if ((int)Session["FormatID"] == 3)
                {
                    if (resim != null)
                    {
                        drpDoldur();
                        var filename = DateTime.Now.ToString("yyMMddHHmmssff") + resim.FileName;
                        var path = Server.MapPath("/Images/Blog/" + filename);
                        resim.SaveAs(path);
                        article.PhotoPath1 = "/Images/Blog/" + filename;
                    }




                    Guid idArt = (Guid)Session["Article"];
                    Article dbArt = db.Articles.Find(idArt);
                    article.AddDate = dbArt.AddDate;
                    dbArt.UpdateDate = DateTime.Now;

                    Preview dbPre = db.Previews.Find(preview.ID);
                    dbPre.ArticleTitle = preview.ArticleTitle;
                    dbPre.ArticleContent = preview.ArticleContent;


                    foreach (var item in CategoryList)
                    {
                        if (item != dbPre.ArticleCategoryID.ToString())
                            dbPre.ArticleCategoryID = new Guid(item);
                    }

                    if (article.PhotoPath1 == null)
                    {
                        article.PhotoPath1 = dbPre.PhotoPath1;
                    }
                    preview.ArticleCategoryID = dbPre.ArticleCategoryID;
                    db.SaveChanges();

                    dbArt.ArticleTitle = preview.ArticleTitle;
                    dbArt.ArticleContent = preview.ArticleContent;
                    dbArt.Url = preview.Url;
                    dbArt.ArticleCategoryID = preview.ArticleCategoryID;
                    db.SaveChanges();
                    drpDoldur();
                    db.Previews.Remove(dbPre);
                    db.SaveChanges();

                    return RedirectToAction("Index");


                }
                if ((int)Session["FormatID"] == 4)
                {
                    if (resim != null)
                    {
                        drpDoldur();
                        var filename = DateTime.Now.ToString("yyMMddHHmmssff") + resim.FileName;
                        var path = Server.MapPath("/Images/Blog/" + filename);
                        resim.SaveAs(path);
                        article.PhotoPath1 = "/Images/Blog/" + filename;
                    }
                    if (resim2 != null)
                    {
                        drpDoldur();
                        var filename = DateTime.Now.ToString("yyMMddHHmmssff") + resim2.FileName;
                        var path = Server.MapPath("/Images/Blog/" + filename);
                        resim2.SaveAs(path);
                        article.PhotoPath2 = "/Images/Blog/" + filename;
                    }
                    if (resim3 != null)
                    {
                        drpDoldur();
                        var filename = DateTime.Now.ToString("yyMMddHHmmssff") + resim3.FileName;
                        var path = Server.MapPath("/Images/Blog/" + filename);
                        resim3.SaveAs(path);
                        article.PhotoPath3 = "/Images/Blog/" + filename;
                    }

                    Guid idArt = (Guid)Session["Article"];
                    Article dbArt = db.Articles.Find(idArt);
                    article.AddDate = dbArt.AddDate;
                    dbArt.UpdateDate = DateTime.Now;


                    Preview dbPre = db.Previews.Find(preview.ID);
                    dbPre.ArticleTitle = preview.ArticleTitle;
                    dbPre.ArticleContent = preview.ArticleContent;


                    foreach (var item in CategoryList)
                    {
                        if (item != dbPre.ArticleCategoryID.ToString())
                            dbPre.ArticleCategoryID = new Guid(item);
                    }

                    if (article.PhotoPath1 == null)
                    {
                        article.PhotoPath1 = dbPre.PhotoPath1;
                    }
                    if (article.PhotoPath2 == null)
                    {
                        article.PhotoPath2 = dbPre.PhotoPath2;
                    }
                    if (article.PhotoPath3 == null)
                    {
                        article.PhotoPath3 = dbPre.PhotoPath3;
                    }
                    preview.ArticleCategoryID = dbPre.ArticleCategoryID;
                    db.SaveChanges();

                    dbArt.ArticleTitle = preview.ArticleTitle;
                    dbArt.ArticleContent = preview.ArticleContent;
                    dbArt.ArticleCategoryID = preview.ArticleCategoryID;
                    db.SaveChanges();
                    drpDoldur();
                    db.Previews.Remove(dbPre);
                    db.SaveChanges();

                    return RedirectToAction("Index");

                }

                if ((int)Session["FormatID"] == 5)
                {
                    if (resim != null)
                    {
                        drpDoldur();
                        var filename = DateTime.Now.ToString("yyMMddHHmmssff") + resim.FileName;
                        var path = Server.MapPath("/Images/Blog/" + filename);
                        resim.SaveAs(path);
                        article.PhotoPath1 = "/Images/Blog/" + filename;
                    }




                    Guid idArt = (Guid)Session["Article"];
                    Article dbArt = db.Articles.Find(idArt);
                    article.AddDate = dbArt.AddDate;
                    dbArt.UpdateDate = DateTime.Now;

                    Preview dbPre = db.Previews.Find(preview.ID);
                    dbPre.ArticleTitle = preview.ArticleTitle;
                    dbPre.ArticleContent = preview.ArticleContent;

                    dbPre.Author = preview.Author;

                    dbPre.QuoteContent = preview.QuoteContent;

                    foreach (var item in CategoryList)
                    {
                        if (item != dbPre.ArticleCategoryID.ToString())
                            dbPre.ArticleCategoryID = new Guid(item);
                    }

                    if (article.PhotoPath1 == null)
                    {
                        article.PhotoPath1 = dbPre.PhotoPath1;
                    }
                    preview.ArticleCategoryID = dbPre.ArticleCategoryID;
                    db.SaveChanges();


                    dbArt.ArticleTitle = preview.ArticleTitle;
                    dbArt.ArticleContent = preview.ArticleContent;
                    dbArt.Author = preview.Author;

                    dbArt.QuoteContent = preview.QuoteContent;
                    dbArt.ArticleCategoryID = preview.ArticleCategoryID;
                    db.SaveChanges();
                    drpDoldur();
                    db.Previews.Remove(dbPre);
                    db.SaveChanges();

                    return RedirectToAction("Index");

                }
                else
                {
                    ViewBag.Durum = -1;
                    return View();
                }
            }

            else
            {
                drpDoldur();

                if ((int)Session["FormatID"] == 1)
                {
                    if (resim != null)
                    {
                        drpDoldur();
                        var filename = DateTime.Now.ToString("yyMMddHHmmssff") + resim.FileName;
                        var path = Server.MapPath("/Images/Blog/" + filename);
                        resim.SaveAs(path);
                        article.PhotoPath1 = "/Images/Blog/" + filename;
                    }



                    Preview dbPre = db.Previews.Find(preview.ID);
                    dbPre.ArticleTitle = preview.ArticleTitle;
                    dbPre.ArticleContent = preview.ArticleContent;
                    Session["Preview"] = id;

                    foreach (var item in CategoryList)
                    {
                        if (item != dbPre.ArticleCategoryID.ToString())
                            dbPre.ArticleCategoryID = new Guid(item);
                    }

                    if (article.PhotoPath1 == null)
                    {
                        article.PhotoPath1 = dbPre.PhotoPath1;
                    }
                    preview.ArticleCategoryID = dbPre.ArticleCategoryID;
                    db.SaveChanges();

                    return RedirectToAction("Detay", new { id = Session["Preview"] });

                }

                if ((int)Session["FormatID"] == 2)
                {
                    if (resim != null)
                    {
                        drpDoldur();
                        var filename = DateTime.Now.ToString("yyMMddHHmmssff") + resim.FileName;
                        var path = Server.MapPath("/Images/Blog/" + filename);
                        resim.SaveAs(path);
                        article.PhotoPath1 = "/Images/Blog/" + filename;
                    }

                    if (music != null)
                    {
                        var fileName = DateTime.Now.ToString("yyMMddHHmmssff") + music.FileName;
                        var path = Server.MapPath("/Music/" + fileName);
                        music.SaveAs(path);
                        article.Audio = "/Music/" + fileName;
                    }

                    Preview dbPre = db.Previews.Find(preview.ID);
                    dbPre.ArticleTitle = preview.ArticleTitle;
                    dbPre.ArticleContent = preview.ArticleContent;
                    Session["Preview"] = id;

                    foreach (var item in CategoryList)
                    {
                        if (item != dbPre.ArticleCategoryID.ToString())
                            dbPre.ArticleCategoryID = new Guid(item);
                    }

                    if (article.PhotoPath1 == null)
                    {
                        article.PhotoPath1 = dbPre.PhotoPath1;
                    }
                    preview.ArticleCategoryID = dbPre.ArticleCategoryID;
                    db.SaveChanges();



                    return RedirectToAction("Detay", new { id = Session["Preview"] });





                }


                if ((int)Session["FormatID"] == 3)
                {
                    if (resim != null)
                    {
                        drpDoldur();
                        var filename = DateTime.Now.ToString("yyMMddHHmmssff") + resim.FileName;
                        var path = Server.MapPath("/Images/Blog/" + filename);
                        resim.SaveAs(path);
                        article.PhotoPath1 = "/Images/Blog/" + filename;
                    }




                    Guid idArt = (Guid)Session["Article"];
                    Article dbArt = db.Articles.Find(idArt);
                    article.AddDate = dbArt.AddDate;
                    dbArt.UpdateDate = DateTime.Now;

                    Preview dbPre = db.Previews.Find(preview.ID);
                    dbPre.ArticleTitle = preview.ArticleTitle;
                    dbPre.ArticleContent = preview.ArticleContent;


                    foreach (var item in CategoryList)
                    {
                        if (item != dbPre.ArticleCategoryID.ToString())
                            dbPre.ArticleCategoryID = new Guid(item);
                    }

                    if (article.PhotoPath1 == null)
                    {
                        article.PhotoPath1 = dbPre.PhotoPath1;
                    }
                    preview.ArticleCategoryID = dbPre.ArticleCategoryID;
                    db.SaveChanges();

                    dbArt.ArticleTitle = preview.ArticleTitle;
                    dbArt.ArticleContent = preview.ArticleContent;
                    dbArt.Url = preview.Url;
                    dbArt.ArticleCategoryID = preview.ArticleCategoryID;
                    db.SaveChanges();
                    drpDoldur();
                    db.Previews.Remove(dbPre);
                    db.SaveChanges();

                    return RedirectToAction("Index");


                }
                if ((int)Session["FormatID"] == 4)
                {
                    if (resim != null)
                    {
                        drpDoldur();
                        var filename = DateTime.Now.ToString("yyMMddHHmmssff") + resim.FileName;
                        var path = Server.MapPath("/Images/Blog/" + filename);
                        resim.SaveAs(path);
                        article.PhotoPath1 = "/Images/Blog/" + filename;
                    }

                    if (resim2 != null)
                    {
                        drpDoldur();
                        var filename = DateTime.Now.ToString("yyMMddHHmmssff") + resim2.FileName;
                        var path = Server.MapPath("/Images/Blog/" + filename);
                        resim2.SaveAs(path);
                        article.PhotoPath2 = "/Images/Blog/" + filename;
                    }
                    if (resim3 != null)
                    {
                        drpDoldur();
                        var filename = DateTime.Now.ToString("yyMMddHHmmssff") + resim3.FileName;
                        var path = Server.MapPath("/Images/Blog/" + filename);
                        resim3.SaveAs(path);
                        article.PhotoPath3 = "/Images/Blog/" + filename;
                    }

                    Preview dbPre = db.Previews.Find(preview.ID);
                    dbPre.ArticleTitle = preview.ArticleTitle;
                    dbPre.ArticleContent = preview.ArticleContent;
                    Session["Preview"] = id;

                    foreach (var item in CategoryList)
                    {
                        if (item != dbPre.ArticleCategoryID.ToString())
                            dbPre.ArticleCategoryID = new Guid(item);
                    }

                    if (article.PhotoPath1 == null)
                    {
                        article.PhotoPath1 = dbPre.PhotoPath1;
                    }
                    preview.ArticleCategoryID = dbPre.ArticleCategoryID;
                    db.SaveChanges();



                    return RedirectToAction("Detay", new { id = Session["Preview"] });

                }

                if ((int)Session["FormatID"] == 5)
                {
                    if (resim != null)
                    {
                        drpDoldur();
                        var filename = DateTime.Now.ToString("yyMMddHHmmssff") + resim.FileName;
                        var path = Server.MapPath("/Images/Blog/" + filename);
                        resim.SaveAs(path);
                        article.PhotoPath1 = "/Images/Blog/" + filename;
                    }




                    Guid idArt = (Guid)Session["Article"];
                    Article dbArt = db.Articles.Find(idArt);
                    article.AddDate = dbArt.AddDate;
                    dbArt.UpdateDate = DateTime.Now;

                    Preview dbPre = db.Previews.Find(preview.ID);
                    dbPre.ArticleTitle = preview.ArticleTitle;
                    dbPre.ArticleContent = preview.ArticleContent;

                    dbPre.Author = preview.Author;

                    dbPre.QuoteContent = preview.QuoteContent;

                    foreach (var item in CategoryList)
                    {
                        if (item != dbPre.ArticleCategoryID.ToString())
                            dbPre.ArticleCategoryID = new Guid(item);
                    }

                    if (article.PhotoPath1 == null)
                    {
                        article.PhotoPath1 = dbPre.PhotoPath1;
                    }
                    preview.ArticleCategoryID = dbPre.ArticleCategoryID;
                    db.SaveChanges();


                    dbArt.ArticleTitle = preview.ArticleTitle;
                    dbArt.ArticleContent = preview.ArticleContent;
                    dbArt.Author = preview.Author;

                    dbArt.QuoteContent = preview.QuoteContent;
                    dbArt.ArticleCategoryID = preview.ArticleCategoryID;
                    db.SaveChanges();
                    drpDoldur();
                    db.Previews.Remove(dbPre);
                    db.SaveChanges();

                    return RedirectToAction("Index");

                }
                else
                {
                    ViewBag.Durum = -1;
                    return View();
                }
                //if (resim != null)
                //{
                //    drpDoldur();
                //    var filename = DateTime.Now.ToString("yyMMddHHmmssff") + resim.FileName;
                //    var path = Server.MapPath("/Images/" + filename);
                //    resim.SaveAs(path);
                //    article.PhotoPath1 = "/Images/" + filename;
                //}



                //Preview dbPre = db.Previews.Find(preview.ID);
                //dbPre.ArticleTitle = preview.ArticleTitle;
                //dbPre.ArticleContent = preview.ArticleContent;
                //Session["Preview"] = id;

                //foreach (var item in CategoryList)
                //{
                //    if (item != dbPre.CategoryID.ToString())
                //        dbPre.CategoryID = Convert.ToInt32(item);
                //}

                //if (article.PhotoPath1 == null)
                //{
                //    article.PhotoPath1 = dbPre.PhotoPath1;
                //}
                //preview.CategoryID = dbPre.CategoryID;
                //db.SaveChanges();



                //return RedirectToAction("Detay", new { id = Session["Preview"] });

            }
        }

        public ActionResult PrevSave1(Guid id)
        {
            if (Request.Cookies["Admin"] != null)
            {
                ArticlePreview _model = new ArticlePreview();
                _model.preview = db.Previews.Find(id);
                Session["Photo"] = _model.preview.PhotoPath1;
                Session["Photo2"] = _model.preview.PhotoPath2;

                Session["Photo3"] = _model.preview.PhotoPath3;

                drpDoldur();
                drpDoldurEdit(_model.preview.ArticleCategoryID);



                return View(_model);
            }
            else
            {
                return Redirect("/Admin/LoginAdmin/Index/");
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PrevSave1(Guid id, Preview preview, Article article, string[] CategoryList, HttpPostedFileBase resim, HttpPostedFileBase resim2, HttpPostedFileBase resim3, string command)
        {
            if (command.Equals("Kaydet"))
            {
                if (resim != null)
                {
                    drpDoldur();
                    var filename = DateTime.Now.ToString("yyMMddHHmmssff") + resim.FileName;
                    var path = Server.MapPath("/Images/Blog/" + filename);
                    resim.SaveAs(path);
                    article.PhotoPath1 = "/Images/Blog/" + filename;
                }



                if (Session["Article"] != null)
                {
                    Guid idArt = (Guid)Session["Article"];
                    Article dbArt = db.Articles.Find(idArt);
                    article.AddDate = dbArt.AddDate;
                    dbArt.UpdateDate = DateTime.Now;

                    Preview dbPre = db.Previews.Find(preview.ID);
                    dbPre.ArticleTitle = preview.ArticleTitle;
                    dbPre.ArticleContent = preview.ArticleContent;


                    foreach (var item in CategoryList)
                    {
                        if (item != dbPre.ArticleCategoryID.ToString())
                            dbPre.ArticleCategoryID = new Guid(item);
                    }

                    if (article.PhotoPath1 == null)
                    {
                        article.PhotoPath1 = dbPre.PhotoPath1;
                    }
                    preview.ArticleCategoryID = dbPre.ArticleCategoryID;
                    db.SaveChanges();

                    dbArt.ArticleTitle = preview.ArticleTitle;
                    dbArt.ArticleContent = preview.ArticleContent;
                    dbArt.ArticleCategoryID = preview.ArticleCategoryID;
                    db.SaveChanges();
                    drpDoldur();
                    db.Previews.Remove(dbPre);
                    db.SaveChanges();
                }
                else
                {
                    article.AddDate = DateTime.Now;
                    article.UpdateDate = DateTime.Now;

                    Preview dbPre = db.Previews.Find(preview.ID);
                    dbPre.ArticleTitle = preview.ArticleTitle;
                    dbPre.ArticleContent = preview.ArticleContent;


                    foreach (var item in CategoryList)
                    {
                        if (item != dbPre.ArticleCategoryID.ToString())
                            dbPre.ArticleCategoryID = new Guid(item);
                    }

                    if (article.PhotoPath1 == null)
                    {
                        article.PhotoPath1 = dbPre.PhotoPath1;
                    }
                    preview.ArticleCategoryID = dbPre.ArticleCategoryID;
                    db.SaveChanges();

                    article.ArticleTitle = preview.ArticleTitle;
                    article.ArticleContent = preview.ArticleContent;
                    article.ArticleCategoryID = preview.ArticleCategoryID;
                    db.Articles.Add(article);
                    db.SaveChanges();
                    drpDoldur();
                    db.Previews.Remove(dbPre);
                    db.SaveChanges();

                    foreach (var item in CategoryList)
                    {
                        ArticleCategoryJoin ac = new ArticleCategoryJoin();
                        ac.ID = Guid.NewGuid();
                        ac.ArticleID = article.ID;
                        ac.ArticleCategoryID = new Guid(item);
                        db.ArticleCategoryJoins.Add(ac);
                        db.SaveChanges();
                    }

                }






                return RedirectToAction("Index");
            }

            else
            {
                if (resim != null)
                {
                    drpDoldur();
                    var filename = DateTime.Now.ToString("yyMMddHHmmssff") + resim.FileName;
                    var path = Server.MapPath("/Images/Blog/" + filename);
                    resim.SaveAs(path);
                    article.PhotoPath1 = "/Images/Blog/" + filename;
                }



                Preview dbPre = db.Previews.Find(preview.ID);
                dbPre.ArticleTitle = preview.ArticleTitle;
                dbPre.ArticleContent = preview.ArticleContent;
                Session["Preview"] = id;

                foreach (var item in CategoryList)
                {
                    if (item != dbPre.ArticleCategoryID.ToString())
                        dbPre.ArticleCategoryID = new Guid(item);
                }

                if (article.PhotoPath1 == null)
                {
                    article.PhotoPath1 = dbPre.PhotoPath1;
                }
                preview.ArticleCategoryID = dbPre.ArticleCategoryID;
                db.SaveChanges();



                return RedirectToAction("Detay", new { id = Session["Preview"] });

            }
        }

        public ActionResult PrevSaved(Guid id, Article article, string[] CategoryList, HttpPostedFileBase resim)
        {
            Preview prev = db.Previews.Find(id);

            if (ModelState.IsValid)
            {

                if (Session["Article"] != null)
                {
                    Guid idArt = new Guid((string)Session["Article"]);
                    Article dbArt = db.Articles.Find(idArt);
                    article.AddDate = dbArt.AddDate;
                    dbArt.UpdateDate = DateTime.Now;

                    Preview dbPre = db.Previews.Find(prev.ID);
                    dbPre.ArticleTitle = prev.ArticleTitle;
                    dbPre.ArticleContent = prev.ArticleContent;



                    if (article.PhotoPath1 == null)
                    {
                        article.PhotoPath1 = dbPre.PhotoPath1;
                    }
                    prev.ArticleCategoryID = dbPre.ArticleCategoryID;
                    db.SaveChanges();

                    dbArt.ArticleTitle = prev.ArticleTitle;
                    dbArt.ArticleContent = prev.ArticleContent;
                    dbArt.ArticleCategoryID = prev.ArticleCategoryID;
                    db.SaveChanges();
                    Session["Article"] = null;

                    db.Previews.Remove(dbPre);
                    db.SaveChanges();

                }
                else
                {
                    article.PhotoPath1 = prev.PhotoPath1;
                    article.AddDate = DateTime.Now;
                    article.UpdateDate = DateTime.Now;
                    article.ArticleTitle = prev.ArticleTitle;
                    article.ArticleContent = prev.ArticleContent;
                    article.ArticleCategoryID = prev.ArticleCategoryID;

                    db.Articles.Add(article);
                    db.SaveChanges();

                    ArticleCategoryJoin ac = new ArticleCategoryJoin();
                    ac.ID = Guid.NewGuid();
                    ac.ArticleID = article.ID;
                    Guid categoryID = (Guid)prev.ArticleCategoryID;
                    db.ArticleCategoryJoins.Add(ac);
                    db.SaveChanges();




                    db.Previews.Remove(prev);
                    db.SaveChanges();


                }






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
            var del = db.Articles.Find(id);
            del.IsDelete = true;
            db.SaveChanges();
            return Json("");

        }

        public ActionResult DateEdit(int id)
        {
            if (Request.Cookies["Admin"] != null)
            {

                Article article = db.Articles.Find(id);


                return View(article);
            }
            else
            {
                return Redirect("/Admin/LoginAdmin/Index/");
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult DateEdit(Article article)
        {
            if (ModelState.IsValid)
            {

                var _article = db.Articles.Find(article.ID);
                article.UpdateDate = DateTime.Now;
                article.ArticleContent = _article.ArticleContent;
                article.ArticleTitle = _article.ArticleTitle;
                article.IsDelete = _article.IsDelete;
                article.PhotoPath1 = _article.PhotoPath1;
                article.ArticleCategoryID = _article.ArticleCategoryID;
                db.Entry(_article).CurrentValues.SetValues(article);
                db.SaveChanges();


                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Durum = -1;
                return View();
            }

        }




        public ActionResult Detay(Guid id)
        {
            Preview prev = db.Previews.Find(id);

            return View(prev);
        }

        public ActionResult Detay1(Guid id)
        {
            Preview prev = db.Previews.Find(id);

            return View(prev);
        }
    }
}