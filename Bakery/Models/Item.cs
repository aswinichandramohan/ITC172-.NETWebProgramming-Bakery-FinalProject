using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bakery.Models
{
    public class Item
    {
        /// <summary>
        /// This object stores all the information
        /// about a particular item in a sale.
        /// I use it to write the salesdetails
        /// to the database
        /// </summary>
        public int ProductKey { set; get; }
        public string ProductName { get; set; }
        public decimal Price { set; get; }
        public int Quantity { set; get; }
        public decimal Discount { set; get; }
    }
}