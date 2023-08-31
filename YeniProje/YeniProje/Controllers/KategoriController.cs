using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;
using System.Web.Mvc;
using YeniProje.Models.Entity;
using YeniProje.Models;


namespace MvcStok.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class KategoriController : Controller
    {
        // GET: Kategori
        MvcDbStokEntities db = new MvcDbStokEntities();
        public ActionResult Index()
        {
            var degerler = db.TBLKATEGORILER.ToList();
            return View(degerler);
        }
        [HttpGet]
        public ActionResult Yenikategori()
        {
            return View();
        }

       
            [HttpPost]
        public ActionResult Yenikategori(TBLKATEGORILER p1)
        {
            if (!ModelState.IsValid)
            {
                return View("YeniKategori");
            }
            db.TBLKATEGORILER.Add(p1);
            db.SaveChanges();
            return View();

        }

        public ActionResult Sil(int id)
        {
            var kategori = db.TBLKATEGORILER.Find(id);
            db.TBLKATEGORILER.Remove(kategori);
            db.SaveChanges();
            return RedirectToAction("Index");

        }
        public ActionResult KategoriGetir(int id)
        {
            var ktgr = db.TBLKATEGORILER.Find(id);
            return View("KategoriGetir", ktgr);
        }
        public ActionResult Guncelle(TBLKATEGORILER p1)
        {
            var ktg = db.TBLKATEGORILER.Find(p1.KATEGORIID);
            ktg.KATEGORIAD = p1.KATEGORIAD;
            db.SaveChanges();
            return RedirectToAction("Index");


        }

    }
}
/*@model YeniProje.Models.UserModel
@{
    ViewBag.Title = "CreateUser";
    Layout = "~/Views/Shared/_Layout.cshtml";
}


<h2>Create New User</h2>

@using (Html.BeginForm("CreateUser", "Admin", FormMethod.Post))
{
    <table class="table table table-bordered">

        <div>
            @Html.LabelFor(model => model.Username)
            @Html.TextBoxFor(model => model.Username)
        </div>
        <br />
        <div>
            @Html.LabelFor(model => model.Email)
            @Html.TextBoxFor(model => model.Email)
        </div>
    </table>

}

*/