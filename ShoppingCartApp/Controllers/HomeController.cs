using Microsoft.AspNet.Identity;
using ShoppingCartApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoppingCartApp.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        [Authorize]

        public ActionResult Index()
        {
            //This brings in the userid
            //var myId = User.Identity.GetUserId();
            //var user = db.Users.Find(myId);
            //var shoppingCarts = db.ShoppingCarts.Where(s => s.CustomerId == user.Id).ToList();

            var items = db.Items.ToList();
            return View(items);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult products()
        {
            return View();
        }

        public ActionResult single()
        {
            return View();
        }

        public ActionResult account()
        {
            return View();
        }

        public ActionResult checkout()
        {
            return View();
        }

        public ActionResult register()
        {
            return View();
        }

        public ActionResult typo()
        {
            return View();
        }

        
    }
}

//a href="@Url.Action("ActionResult that you want rendered")"><img src="/picture"/></> This will put a picture up
// that you can click on and it will take you to the ActionResult that you want it direct you to.