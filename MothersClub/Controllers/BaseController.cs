using MothersClub.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MothersClub.Controllers
{
    [CheckSessionOut]
    [Authorize]
    public class BaseController : Controller
    {

    }
}