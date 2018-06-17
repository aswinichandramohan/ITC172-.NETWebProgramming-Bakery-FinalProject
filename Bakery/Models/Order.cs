using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bakery.Models
{
    public class Order
    {
       
        List<Item> items;
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }

        public decimal Total { set; get; }
        public decimal SubTotalAfterDiscount { set; get; }


        public decimal Discount { set; get; }

        //construtor
        public Order()
        {
            //initialize the list object
            items = new List<Item>();
        }

        public void AddItem(Item i)
        {
            //add items to list
            items.Add(i);
        }

        public List<Item> GetItems()
        {
            //get the list
            return items;
        }

        public void CalculateSubTotal()
        {
            //loops through the items
            //to get the subtotal
            decimal sum = 0;

            foreach (Item i in items)
            {
                sum += i.Price * i.Quantity;

            }
            SubTotal = sum;
        }

        public void CalculateDiscount()
        {
            //loops through the items
            //to calculate total discounts
            decimal discount = 0;
            foreach (Item i in items)
            {
                discount += i.Price * i.Quantity * i.Discount;
            }
            Discount = discount;
        }

        public void CalculateSubAfterDiscount()
        {
            //subtracts discounts from subtotal
            SubTotalAfterDiscount = SubTotal - Discount;
        }
        public void CalculateTax()
        {
            //calculates tax
            decimal tax = 0M;
            tax = SubTotal * .09M;
            Tax = tax;

        }

        public void CalculateTotal()
        {
            //calculates the total
            Total = SubTotalAfterDiscount + Tax;
        }
    }
}