using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bakery.Models;

namespace Bakery.Controllers
{
    /// <summary>
    /// This controller is complex. It uses the Item and the Order classes.
    /// The order form uses two buttons: One to add the items
    /// and another to complete the order.
    /// The Index process the items, adding them to the list.
    /// The finsihOrder method writes the sale table info
    /// and all the saledetail info for the sale
    /// </summary>
    public class OrderController : Controller
    {
        BakeryEntities db = new BakeryEntities();

        // GET: Order
        public ActionResult Index()
        {
            if (Session["PersonKey"] == null)
            {
                Message m = new Message();
                m.MessageText = "You must be logged in to order";
                return RedirectToAction("Result", m);
            }
            //create the Order object
            Order o = new Order();
            //I write the order object to a session so it can retain its values
            //otherwise the order refreshes each time the form is used
            //and loses all its values
            Session["orders"] = o;
            //viewbag to send product list for dropdown in form
            ViewBag.products = new SelectList(db.Products, "ProductKey", "ProductName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "ProductKey,ProductName,Price,Quantity, Discount")]Item i)
        {
            //for the reciept I want to populate the names and prices
            //of the products, so I do a query to get those fields
            //from the database based on the productkey,
            //then I loop through the results and write them to the Item
            //object
            var prod = from p in db.Products
                       where p.ProductKey == i.ProductKey
                       select new { p.ProductName, p.ProductPrice };

            foreach (var pr in prod)
            {
                i.ProductName = pr.ProductName.ToString();
                i.Price = (decimal)pr.ProductPrice;
            }
            //get the order back from the Session.
            //This is not ideal because it involves a lot of server
            //traffic
            Order o = (Order)Session["Orders"];
            //add the item to the Items list in the Order class
            o.AddItem(i);
            //write the object back to the session
            Session["Orders"] = o;
            //Since we are using the form more than once in a single session
            //need to refresh the viewbag
            ViewBag.products = new SelectList(db.Products, "ProductKey", "ProductName");
            return View();
        }

        public ActionResult FinishOrder()
        {
            //here I want to write the sale and saledetail to the database
            //and then pass the order onto the Reciept view
            Sale sale = new Sale();
            sale.EmployeeKey = 1;
            sale.SaleDate = DateTime.Now;
            sale.CustomerKey = (int)Session["PersonKey"];
            db.Sales.Add(sale);

            //get the order back from the Session
            Order o = (Order)Session["orders"];
            //get the items list from the order object
            List<Item> saleItems = o.GetItems();
            //Loop through them and write the values
            //to the SaleDetails object
            foreach (Item i in saleItems)
            {
                SaleDetail sd = new SaleDetail();
                sd.Sale = sale;
                sd.ProductKey = i.ProductKey;
                sd.SaleDetailPriceCharged = i.Price;
                sd.SaleDetailQuantity = i.Quantity;
                sd.SaleDetailDiscount = (decimal)i.Discount;
                sd.SaleDetailSaleTaxPercent = .09m;
                sd.SaleDetailEatInTax = .01m;

                db.SaleDetails.Add(sd);
            }
            //save all the changes
            db.SaveChanges();

            //Make sure all the calculations are done
            //before passing the Orders object
            //to the reciept
            o.CalculateSubTotal();
            o.CalculateDiscount();
            o.CalculateSubAfterDiscount();
            o.CalculateTax();
            o.CalculateTotal();

            return View("Receipt", o);
        }

        public ActionResult Receipt(Order order)
        {
            return View(order);
        }

        public ActionResult Result(Message m)
        {
            return View(m);
        }
    }
}