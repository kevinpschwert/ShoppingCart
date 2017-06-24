using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ShoppingCartApp.Models;
using Microsoft.AspNet.Identity;

namespace ShoppingCartApp.Controllers
{
    public class ShoppingCartsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ShoppingCarts
        public ActionResult Index()
        {
            //var myId = User.Identity.GetUserId();
            //var user = db.Users.Find(myId);
            
            //This code presupposes that the user is logged in. User.Identity.GetUserId acts upon a logged in user...
            var user = db.Users.Find(User.Identity.GetUserId());
            //If the user is not logged in this line of code will throw an exception because user.Id is null
            
            var shoppingCarts = db.ShoppingCarts.Where(s => s.CustomerId == user.Id).ToList();
            Order order = new Order();
            ShoppingCart shoppingCart = new ShoppingCart();            
            shoppingCart.CustomerId = user.Id;

            var count = shoppingCarts.Count;

                if (count == 0)
                {
                ViewBag.Message = 0;
                }
                else
                {
                ViewBag.num = db.ShoppingCarts.Where(x => x.CustomerId == user.Id).Sum(m => m.Item.Price * m.Count);
                ViewBag.total = ViewBag.num + 150; ;


                //ViewBag.num = db.ShoppingCarts.Sum(m => m.Item.Price * m.Count);
                //ViewBag.total = ViewBag.num + 150;
                TempData["TotalPrice"] = ViewBag.total;
                }
                return View(shoppingCarts);
            
            //return View(db.ShoppingCarts.ToList());
        }

        // GET: ShoppingCarts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShoppingCart shoppingCart = db.ShoppingCarts.Find(id);
            if (shoppingCart == null)
            {
                return HttpNotFound();
            }
            return View(shoppingCart);
        }

        //GET: ShoppingCarts/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        // POST: ShoppingCarts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,ItemId,CustomerId,Count,Created")] ShoppingCart shoppingCart)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.ShoppingCarts.Add(shoppingCart);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(shoppingCart);
        //}
        //[Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int itemid)
        {

            var user = db.Users.Find(User.Identity.GetUserId());

            //If the user is not logged in this line of code will throw an exception because user.Id is null
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var exShopping = db.ShoppingCarts.Where(s => s.CustomerId == user.Id && s.ItemId == itemid).ToList();

            //If and only if the Users cart does not already have this particular item we will execute this code
            if (exShopping.Count == 0)
            {
                ShoppingCart shoppingCart = new ShoppingCart();
                shoppingCart.CustomerId = user.Id;
                shoppingCart.ItemId = itemid;
                shoppingCart.Item = db.Items.FirstOrDefault(i => i.Id == itemid);
                shoppingCart.Count = 1;
                shoppingCart.Created = System.DateTime.Now;                
                db.ShoppingCarts.Add(shoppingCart);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            //Otherwise we will execute this code block which will simply increment the item count
            foreach (var items in exShopping)
            {
                items.Count++;
                db.Entry(items).Property("Count").IsModified = true;
            };

            db.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        // GET: ShoppingCarts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShoppingCart shoppingCart = db.ShoppingCarts.Find(id);
            if (shoppingCart == null)
            {
                return HttpNotFound();
            }
            return View(shoppingCart);
        }

        // POST: ShoppingCarts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ItemId,Count,Created,CustomerId")] ShoppingCart shoppingCart)
        {
            var items = db.Items.ToList();
            var user = db.Users.Find(User.Identity.GetUserId());

            shoppingCart.CustomerId = user.Id;
            if (ModelState.IsValid)
            {
                db.Entry(shoppingCart).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(shoppingCart);
        }

        // GET: ShoppingCarts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShoppingCart shoppingCart = db.ShoppingCarts.Find(id);
            if (shoppingCart == null)
            {
                return HttpNotFound();
            }
            return View(shoppingCart);
        }

        // POST: ShoppingCarts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
             
            ShoppingCart shoppingCart = db.ShoppingCarts.Find(id);
            db.ShoppingCarts.Remove(shoppingCart);
            db.SaveChanges();
            return RedirectToAction("Index");

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        public ActionResult Quantity(int Itemid, FormCollection collection)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var exShopping = db.ShoppingCarts.Where(s => s.CustomerId == user.Id && s.ItemId == Itemid).ToList();

            foreach (var items in exShopping)
            {
                var value = collection["quantity"];
                items.Count = Int32.Parse(value);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }


        [Authorize]
        public PartialViewResult PartialView()
        {

            //This code presupposes that the user is logged in. User.Identity.GetUserId acts upon a logged in user...
            //This can only be called inside a Controller. The only the following statment will work is if they are authorized.
            //
            var user = db.Users.Find(User.Identity.GetUserId());
            //If the user is not logged in this line of code will throw an exception because user.Id is null
            //var orderdetails = db.OrderDetails.ToList(); <--- This gets ALL the OrderDetails. To limit the scope use a Lamda expression
            var shoppingCarts = db.ShoppingCarts.Where(s => s.CustomerId == user.Id);
            if (shoppingCarts.Count() == 0)
            {
                ViewBag.num = 0;
                ViewBag.count = 0;
            }
            else
            {
                //ViewBag.num = db.ShoppingCarts.Sum(m => m.Item.Price * m.Count);
                if (shoppingCarts.Count() != 0)
                {
                    ViewBag.num = db.ShoppingCarts.Where(x => x.CustomerId == user.Id).Sum(m => m.Item.Price * m.Count);
                    ViewBag.count = db.ShoppingCarts.Where(x => x.CustomerId == user.Id).Sum(m => m.Count);
                }
                //ViewBag.count = db.ShoppingCarts.Sum(m => m.Count);
            }
            return PartialView("/Views/Shared/PartialView.cshtml");            
        }

        [HttpPost]
        public ActionResult Checkout()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            //If the user is not logged in this line of code will throw an exception because user.Id is null

            var shoppingCarts = db.ShoppingCarts.Where(s => s.CustomerId == user.Id).ToList();
            ShoppingCart shoppingCart = new ShoppingCart();
            shoppingCart.CustomerId = user.Id;
            Order order = new Order();
            order.Total = Convert.ToDecimal(TempData["TotalPrice"]);
            return View(db.ShoppingCarts.ToList());
        }

       
    }
}
