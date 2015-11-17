using MyBankMVC15.Service;
using MyBankMVC15.Models;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace MyBankMVC15.Controllers
{
    [HandleError]
    public class AuthController : Controller
    {
        public IAuthenticationService AuthService { get; set; }
        // public IMyMembershipService MyMembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (AuthService == null)
                AuthService = GenericFactory<AuthenticationService, IAuthenticationService>.CreateInstance();
           
            base.Initialize(requestContext);
        }

        public ActionResult Login()
        {
            LoginModel model = new LoginModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (AuthService.SignIn(model.Username, model.Password, false))
                {
                    if (!String.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult LogOff()
        {
            AuthService.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ChangePwd()
        {
            UpdatePassword model = new UpdatePassword();
            return View(model);
        }

        [HttpPost]
        public ActionResult ChangePwd(UpdatePassword model)
        {
            if (ModelState.IsValid)
            {
                string oldPwd = Utils.StripPunctuation(model.oldPassword);
                string newPwd = Utils.StripPunctuation(model.newPassword);
                string rePwd = Utils.StripPunctuation(model.reNewPassword);

                if (rePwd.Equals(newPwd))
                {
                    string userName = Utils.StripPunctuation(HttpContext.User.Identity.Name);

                    if (AuthService.ValidateUser(userName, oldPwd))
                    {
                        if (AuthService.ChangePassword(userName, oldPwd, newPwd))
                            model.Status = "Password udpated successfully.";
                        else
                            model.Status = "Couldn't change password!!!";
                    }
                    else
                    {
                        model.Status = "Invalid old password ...";
                    }
                }
                else
                {
                    model.Status = "New passwords mismatch ...";
                }
            }
            return View(model);
        }
    }
}