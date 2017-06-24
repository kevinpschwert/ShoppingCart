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
    public class OrdersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Orders
        public ActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            //If the user is not logged in this line of code will throw an exception because user.Id is null

            var order = db.Orders.Where(s => s.CustomerId == user.Id).ToList();
            ShoppingCart shoppingCart = new ShoppingCart();
            //shoppingCart.CustomerId = user.Id;
            //Item item = new Item();
            Order orders = new Order();
            //ViewBag.num = db.ShoppingCarts.Where(x => x.CustomerId == user.Id).Sum(m => m.Item.Price * m.Count);
            //var bob = db.ShoppingCarts.Where(x => x.CustomerId == user.Id).Sum(m => m.Item.Price * m.Count);
            //ViewBag.total = bob + 150;

            orders.Total = Convert.ToDecimal(TempData["TotalPrice"]);            
            return View(order);
        }

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Orders/Create
        public ActionResult Create(int? Itemid)
        {

            var items = db.ShoppingCarts.ToList();
            //If the user is not logged in this line of code will throw an exception because user.Id is null
            var user = db.Users.Find(User.Identity.GetUserId());

            
            //var orders = db.ShoppingCarts.Where(s => s.CustomerId == user.Id).ToList();
            //ShoppingCart shoppingCart = new ShoppingCart();
            //Item item = new Item();
            //shoppingCart.CustomerId = user.Id;           

            ViewBag.total = TempData["TotalPrice"];
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Completed,Address,City,State,Zipcode,Country,Phone,OrderDate,Total")] Order order)
        {
            //var user = db.Users.Find(User.Identity.GetUserId());

            //if (ModelState.IsValid)
            //{
            //    order.CustomerId = user.Id;                
            //    order.OrderDate = DateTime.Now;
            //    ViewBag.total = TempData["TotalPrice"];
            //    db.Orders.Add(order);
            //    db.SaveChanges();
            //    return RedirectToAction("Index");
            //}

            //return View(order);

            var user = db.Users.Find(User.Identity.GetUserId());
            var shoppingcart = db.ShoppingCarts.Where(s => s.CustomerId == user.Id).ToList();
            decimal totalAmt = 0;
            if (shoppingcart.Count != 0)
            {
                if (ModelState.IsValid)
                {
                    foreach (var product in shoppingcart)
                    {
                        OrderDetail orderdetail = new OrderDetail();
                        orderdetail.ItemId = product.ItemId;
                        orderdetail.OrderId = order.Id;
                        orderdetail.Quantity += product.Count;
                        orderdetail.UnitPrice = product.Item.Price;
                        totalAmt += (product.Count * product.Item.Price);

                        db.OrderDetails.Add(orderdetail);
                    }

                    order.Total = totalAmt + 150;
                    order.Completed = false;
                    order.OrderDate = DateTime.Now;
                    order.CustomerId = user.Id;
                    db.Orders.Add(order);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            ViewBag.noitem = "There's no item";
            return View();
        }
          

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Completed,Address,City,State,Zipcode,Country,Phone,OrderDate,Total,CustomerId")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                order.Completed = true;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(order);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
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
        public ActionResult Done(int? id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Order order = db.Orders.Find(id);
            var shoppingcart = db.ShoppingCarts.Where(s => s.CustomerId == user.Id).ToList();
            var orders = db.Orders.Where(s => s.CustomerId == user.Id).ToList();

            //if (order == null)
            //{
            //    return HttpNotFound();
            //}
            ShoppingCart shoppingCart = db.ShoppingCarts.Find(id);
            //db.ShoppingCarts.Remove();
                var thisorder = db.Orders.Find(id);
                db.Orders.Remove(thisorder);
                db.ShoppingCarts.RemoveRange(shoppingcart);
                db.SaveChanges();            
                    
            return View();
        }
    }
}
