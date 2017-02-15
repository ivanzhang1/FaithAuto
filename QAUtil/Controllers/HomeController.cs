using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QAUtil.Controllers {
    [HandleError]
    public class HomeController : Controller {
        public ActionResult Index() {
            ViewData["Message"] = "Your in-progress, one-stop-shop for all(some) of your QA needs." + "<br /><h3>" + "This application allows you to perform various actions and monitoring for the DEV, QA, and STAGING environments." + "</h3>";

            return View();
        }

        public ActionResult About() {
            return View();
        }
    }
}
