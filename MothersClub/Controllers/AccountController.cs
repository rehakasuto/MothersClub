using MothersClub.Models.ViewModel;
using System;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using MothersClub.Service;

namespace MothersClub.Controllers
{
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (String.IsNullOrEmpty(HttpContext.User.Identity.Name))
            {
                FormsAuthentication.SignOut();
                return View();
            }
            return Redirect("/Home/Index");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginViewModel model, string returnurl)
        {
            if (ModelState.IsValid)
            {
                if (WebConfigurationManager.AppSettings["ae"] != null && WebConfigurationManager.AppSettings["ap"] != null)
                {
                    var aeMatch = model.Email.HashMD5().Equals(WebConfigurationManager.AppSettings["ae"], StringComparison.InvariantCulture);
                    var apMatch = model.Password.HashMD5().Equals(WebConfigurationManager.AppSettings["ap"], StringComparison.InvariantCulture);
                    if (aeMatch && apMatch)
                    {
                        Session["IsAdmin"] = true;
                        Session["UserName"] = model.Email;
                        FormsAuthentication.SetAuthCookie(model.Email, true);
                        return RedirectToAction("Index", "Home");
                    }
                }
                //todo: karşı tarafa istekte bulun gelen sonuca göre session doldur.
                if (model.Email == "rehakasuto@gmail.com" && model.Password == "201990")
                {
                    Session["IsAdmin"] = false;
                    Session["UserName"] = model.Email;
                    Session["CurrentUserId"] = 118;
                    FormsAuthentication.SetAuthCookie(model.Email, true);
                    return RedirectToAction("Index", "Home");
                }
                else if (model.Email == "serayguray@gmail.com" && model.Password == "h2o")
                {
                    Session["IsAdmin"] = false;
                    Session["UserName"] = model.Email;
                    Session["CurrentUserId"] = 120;
                    FormsAuthentication.SetAuthCookie(model.Email, true);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "EMail veya şifre hatalı!");
                }
            }
            return View(model);
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            ClearSession();
            return RedirectToAction("Login", "Account");
        }
        private void ClearSession()
        {
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
        }
    }
}