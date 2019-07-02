using MothersClub.Models;
using MothersClub.Service;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MothersClub.Controllers
{
    public class UserReferenceAdminController : Controller
    {
        // GET: UserReferenceAdmin
        public ActionResult Index()
        {
            if (Session["IsAdmin"] != null && (bool)Session["IsAdmin"] == false)
            {
                return Redirect("/Home/Index");
            }
            ViewBag.IsAdmin = Session["IsAdmin"];
            ViewBag.UserName = Session["UserName"];
            return View();
        }


        [HttpGet]
        public ActionResult RetrieveUserReferences()
        {
            try
            {
                using (MCContext ctx = new MCContext())
                {
                    var userReferences = ctx.userReferences
                                            .OrderByDescending(x => x.referenceUserId)
                                            .ThenByDescending(x => x.id)
                                            .Select(x => new
                                            {
                                                x.totalDiscount,
                                                x.status,
                                                x.createdDate,
                                                x.userInvitation.referenceUserName,
                                                x.userInvitation.mailAddress
                                            })
                                            .ToList();
                    return Json(userReferences, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ActivityService.LogException(ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "İşlem sırasında bir hata oluştu.");
            }
        }
    }
}