using Accounting.Models.reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Accounting.Controllers
{
    public class ReportsController : Controller
    {
        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ClimNotSubmitted()
        {
            return View(new SearchCriteria() { climbNotSub = true });
        }
    }
}