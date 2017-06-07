using Accounting.AccountAuthorize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Accounting.Areas.admin.Controllers
{
    public class PanelController : Controller
    {
        [AccountAutherize(Roles = "admin")]
        // GET: admin/Panel
        public ActionResult Index()
        {
            return View();
        }
    }
}