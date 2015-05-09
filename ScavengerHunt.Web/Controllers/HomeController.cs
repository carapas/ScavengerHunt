using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace ScavengerHunt.Web.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
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

        public ActionResult Doc()
        {
            return View();
        }

        public ActionResult Stats()
        {
            return View();
        }

        public ActionResult Rules()
        {
            return View();
        }

        public ActionResult PartialNavBar()
        {
            // Get current user
            var userid = User.Identity.GetUserId();
            var user = db.Users.Find(userid);
            return PartialView(user);
        }
    }
}