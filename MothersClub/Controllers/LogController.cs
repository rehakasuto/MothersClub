using MothersClub.Models;
using MothersClub.Service;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MothersClub.Controllers
{
    public class LogController : BaseController
    {
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
        public ActionResult RetrieveSystemLogs()
        {
            try
            {
                using (MCContext ctx = new MCContext())
                {
                    var systemLogs = ctx.systemLogs.Where(x => x.userId != null).OrderByDescending(x => x.createdDate).Take(25).ToList();
                    return Json(systemLogs, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ActivityService.LogException(ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "İşlem sırasında bir hata oluştu.");
            }
        }
        [HttpGet]
        public ActionResult RetrieveExceptionLogs()
        {
            try
            {
                using (MCContext ctx = new MCContext())
                {
                    var exceptionLogs = ctx.exceptionLogs.OrderByDescending(x => x.createdDate).Take(10).ToList();
                    return Json(exceptionLogs, JsonRequestBehavior.AllowGet);
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