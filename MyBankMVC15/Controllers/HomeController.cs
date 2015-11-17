using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MyBankMVC15.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "admin")]  //
        public ActionResult News()
        {
            ViewBag.Username = HttpContext.User.Identity.Name;
            return View();
        }
    }
}