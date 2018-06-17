using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bakery.Models;

namespace Bakery.Controllers
{
    public class LoginController : Controller
    {
       
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "UserName, Password")]LoginClass l)
        {
            BakeryEntities db = new BakeryEntities();

            //use the stored procedure to validate the user
            int result = db.usp_Login(l.UserName, l.Password);

            //Initialize the Message object
            Message m = new Message();
            //if the login is valid get the personkey for the user
            if (result != -1)
            {
                var userkey = (from p in db.People
                               where p.PersonEmail.Equals(l.UserName)
                               select p.PersonKey).FirstOrDefault();

                int pkey = (int)userkey;
                //write the personkey to a session
                Session["PersonKey"] = pkey;

                //Create the welcome message
                m.MessageText = "Welcome, " + l.UserName;

            }
            else
            {
                //if not valid send the the invalid login message
                m.MessageText = "Invalid Login";
            }

            return View("Result", m);
        }

        public ActionResult Result(Message m)
        {
            return View(m);
        }
    }
}