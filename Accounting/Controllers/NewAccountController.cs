using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Accounting.Controllers
{
    public class NewAccountController : Controller
    {
        // GET: NewAccount
        public ActionResult Index()
        {
            return View();
        }
    }
}