using MyBankMVC15.Business;
using MyBankMVC15.Models;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace MyBankMVC15.Controllers
{
    [HandleError]
    public class AuthController : Controller
    {
        public IBusinessAuthentication AuthService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (AuthService == null)
                AuthService = GenericFactory<BusinessLayer, IBusinessAuthentication>.CreateInstance();
           
            base.Initialize(requestContext);
        }

        public ActionResult Login()
        {
            LoginModel model = new LoginModel();
            model.Username = HttpContext.User.Identity.Name;
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

        [Authorize]
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
                    if (oldPwd.Equals(newPwd))
                    {
                        model.Status = "Current password and new passwords are same!!!";
                    }
                    else
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