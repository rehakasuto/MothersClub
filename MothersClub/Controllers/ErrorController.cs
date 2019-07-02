using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MothersClub.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            return View("~/Views/Error/PageNotFound.cshtml");
        }
        public ActionResult PageNotFound()
        {
            return View("~/Views/Error/PageNotFound.cshtml");
        }

        public ActionResult InternalServerError()
        {
            return View("~/Views/Error/InternalServerError.cshtml");
        }

        public ActionResult Unauthorized()
        {
            return View("~/Views/Error/Unauthorized.cshtml");
        }
        [AllowAnonymous]
        public ActionResult BadRequest()
        {
            return View("~/Views/Error/BadRequest.cshtml");
        }
    }
}