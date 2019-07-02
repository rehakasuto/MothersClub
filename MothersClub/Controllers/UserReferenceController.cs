using MothersClub.Models;
using MothersClub.Service;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MothersClub.Controllers
{
    public class UserReferenceController : BaseController
    {
        // GET: UserReference
        public ActionResult Index()
        {
            if (Session["IsAdmin"] != null && (bool)Session["IsAdmin"] == true)
            {
                return Redirect("/Home/Index");
            }
            else
            {
                if (Session["CurrentUserId"] == null)
                {
                    Response.Redirect("~/Account/Login");
                }
            }
            ViewBag.IsAdmin = Session["IsAdmin"];
            ViewBag.UserName = Session["UserName"];
            return View();
        }

        #region Rest
        [HttpGet]
        public ActionResult RetrieveUserReferences()
        {
            try
            {
                var userId = Convert.ToInt32(Session["CurrentUserId"]);
                using (MCContext ctx = new MCContext())
                {
                    var userReferences = ctx.userReferences
                                            .Where(x => x.referenceUserId == userId)
                                            .OrderByDescending(x => x.id)
                                            .Select(x => new
                                            {
                                                x.userInvitation.mailAddress,
                                                x.totalDiscount,
                                                x.status,
                                                x.createdDate,
                                                orders = x.orders.Where(a => a.orderState == Enums.OrderState.paid).Select(y => new
                                                {
                                                    y.activationDate,
                                                    y.deservedDiscount
                                                })
                                            }).ToList();
                    return Json(userReferences, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ActivityService.LogException(ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "İşlem sırasında bir hata oluştu.");
            }
        }
        #endregion
    }
}