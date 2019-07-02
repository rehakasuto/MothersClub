using MothersClub.Models;
using MothersClub.Service;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MothersClub.Controllers
{
    public class HomeController : BaseController
    {
        // GET: Home
        public ActionResult Index()
        {
            ViewBag.IsAdmin = Session["IsAdmin"];
            ViewBag.UserName = Session["UserName"];
            return View();
        }

        [HttpGet]
        public ActionResult RetrieveAdminHomeDatas()
        {
            try
            {
                using (MCContext ctx = new MCContext())
                {
                    var portalUserCount = ctx.systemLogs.Where(x => x.userId != null).GroupBy(x => x.userId).Count();
                    var referenceUserCount = ctx.userReferences.Where(x => x.status == Enums.ReferenceStatus.active).Count();
                    return Json(new { portalUserCount, referenceUserCount }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ActivityService.LogException(ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "İşlem sırasında bir hata oluştu.");
            }
        }

        [HttpGet]
        public ActionResult RetrieveWebUserHomeDatas()
        {
            try
            {
                if (Session["CurrentUserId"] == null)
                {
                    Response.Redirect("~/Account/Login");
                }
                var userId = Convert.ToInt32(Session["CurrentUserId"]);
                using (MCContext ctx = new MCContext())
                {
                    var userInvitationCount = ctx.userInvitations.Where(x => x.referenceUserId == userId && x.invitationStatus == Enums.InvitationStatus.delivered).Count();
                    var userReferences = ctx.userReferences
                                            .Where(x => x.referenceUserId == userId)
                                            .Include(x => x.orders);
                    var now = DateTime.Now;

                    var userReferenceCount = userReferences.Where(x => x.status == Enums.ReferenceStatus.active).Count();
                    var allUserReferenceCount = userReferences.Count();
                    decimal referenceTotalDiscount = 0;
                    decimal referenceWaitingDiscount = 0;
                    var orders = userReferences.SelectMany(x => x.orders).Where(x => x.activationDate > now).ToList();
                    if (orders.Any())
                    {
                        referenceWaitingDiscount = orders.Sum(x => x.deservedDiscount);
                    }
                    if (userReferenceCount > 0)
                    {
                        referenceTotalDiscount = userReferences.Sum(x => x.totalDiscount);
                    }
                    var userReferenceOrderCount = userReferences.SelectMany(x => x.orders).Where(x => x.orderState == Enums.OrderState.paid).Count();
                    return Json(new { userInvitationCount, userReferenceCount, referenceTotalDiscount, userReferenceOrderCount, allUserReferenceCount, referenceWaitingDiscount }, JsonRequestBehavior.AllowGet);
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