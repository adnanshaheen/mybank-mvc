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
        private IBusinessAccount _business { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (_business == null)
                _business = GenericFactory<IBusinessAccount, IBusinessAccount>.CreateInstance();

            base.Initialize(requestContext);
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
            model.AccountNumber = _business.GetCheckingAccount(HttpContext.User.Identity.Name);
            model.CheckingBalance = _business.GetCheckingBalance(model.AccountNumber);
            model.SavingBalance = _business.GetSavingBalance(model.AccountNumber + "1");
            return View(model);
        }

        [HttpPost]
        public ActionResult XferChkToSav(XferChkToSavModel model)
        {
            if (model.CheckingBalance < model.AmountTransfer)
                model.Status = "Not enough balance ...";
            else
            {
                if (_business.TransferFromChkgToSav(model.AccountNumber, model.AccountNumber + "1", model.AmountTransfer))
                    model.Status = "amount transfered successfully ...";
                else
                    model.Status = "Couldn't transfer your amount ...";
            }
            return View(model);
        }

        [Authorize]
        public ActionResult XferHistory()
        {
            string AccountNumber = _business.GetCheckingAccount(HttpContext.User.Identity.Name);
            if (AccountNumber == "")
                return View();

            List<TransferHistory> TList = _business.GetTransferHistory(AccountNumber);
            return View(TList);
        }
    }
}