using MyBankMVC15.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace MyBankMVC15.Controllers
{
    public class HomeController : Controller
    {
        private IBusinessAbstraction _business { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            _business = GenericFactory<BusinessAbstraction, IBusinessAbstraction>.CreateInstance();
        }

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

        [Authorize(Roles = "customer")]
        public ActionResult XferChkToSav()
        {
            XferChkToSavModel model = new XferChkToSavModel();
            string ChkAcctNum = _business.GetCheckingAccount(HttpContext.User.Identity.Name);
            model.CheckingBalance = _business.GetCheckingBalance(ChkAcctNum);
            model.SavingBalance = _business.GetSavingBalance(ChkAcctNum + "1");
            return View(model);
        }
    }
}