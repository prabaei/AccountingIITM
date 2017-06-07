using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Accounting.Models.CoordinatorMaster;
using Accounting.data.Database.FACCT;
using Accounting.data.Database.FoxOffice;
using System.Data;
using System.Data.SqlClient;
using Accounting.AccountAuthorize;

namespace Accounting.Controllers
{
    
    public class CoordinatorController : Controller
    {
        readonly IFacctService _ifacctservice;
        readonly IfoxOfficeService _Ifoxofficeservice;
        public CoordinatorController(IFacctService faccountservice, IfoxOfficeService Ifoxofficeservice)
        {
            _ifacctservice = faccountservice;
            _Ifoxofficeservice = Ifoxofficeservice;
            _Ifoxofficeservice.openconnection();
            _ifacctservice.openconnection();
        }

        [AccountAutherize]
        // GET: Coordinator
        public ActionResult Index()
        {
            return View();
        }
        [AccountAutherize]
        public ActionResult Details()
        {

            return View(new CoordModel());
        }
        [AccountAutherize]
        [HttpPost]
        public ActionResult Details(CoordModel mdl)
        {
            return null;
        }
        [AccountAutherize]
        public ActionResult coordinatordetail()
        {
            return PartialView("_coordinatordetail");
        }
        [AccountAutherize]
        [HttpPost]
        public ActionResult coordinatordetail(Search fm)
        {
            
            if (ModelState.IsValid)
            {
                DataTable dttable = new DataTable();
                SqlDataReader Sdr = _Ifoxofficeservice.getmasterdetailbycmd(getCoordinatrDetailCmd(fm.IIRNO));
                dttable.Load(Sdr);
                var coordnatorlist = (from DataRow dr in dttable.Rows
                                      select new
                                      {
                                          Department = Convert.ToString(dr["DEPT"]),
                                          Name = Convert.ToString(dr["NAME"]),
                                          IIRNO = Convert.ToString(dr["IIRNO"]),
                                          DESIG = Convert.ToString(dr["DESIG"])
                                      }).ToList();
                if (coordnatorlist.Count > 1 || coordnatorlist.Count == 0)
                {
                    ///error alert
                }
                else
                {

                    return View("Details", new CoordModel() {
                        Name = coordnatorlist.FirstOrDefault().Name,
                        IIRNO = coordnatorlist.FirstOrDefault().IIRNO,
                        Department = coordnatorlist.FirstOrDefault().Department,
                        Designation = coordnatorlist.FirstOrDefault().DESIG,
                        showprojectdetail = true
                        
                    });
                    
                   

                }

            }
            else
            {
                
                return View("Details", new CoordModel() );
            }
            return View("Details", new CoordModel());
        }
        [AccountAutherize]
        public ActionResult openProjectList(string IIRNO)
        {
           SqlDataReader Sdr = _ifacctservice.FetchfromFacc(getCommandMstlst(IIRNO));
            DataTable dttable = new DataTable();
            dttable.Load(Sdr);
            var projectList = (from DataRow dr in dttable.Rows
                               where Convert.ToDateTime(dr["CLOSE_DATE"]) > DateTime.Now
                               select new projectList
                               {
                                   NPRNO = Convert.ToString(dr["NPRNO"]),
                                   START_DATE = Convert.ToDateTime(dr["START_DATE"]),
                                   CLOSE_DATE = Convert.ToDateTime(dr["CLOSE_DATE"]),
                               }).ToList();
            return PartialView("_openProjectList", projectList as List<projectList>);
        }
        [AccountAutherize]
        public ActionResult ClosedProjectList(string IIRNO)
        {
            SqlDataReader Sdr = _ifacctservice.FetchfromFacc(getCommandMstlst(IIRNO));
            DataTable dttable = new DataTable();
            dttable.Load(Sdr);
            var projectList = (from DataRow dr in dttable.Rows
                               where Convert.ToDateTime(dr["CLOSE_DATE"]) < DateTime.Now
                               select new projectList
                               {
                                   NPRNO = Convert.ToString(dr["NPRNO"]),
                                   START_DATE = Convert.ToDateTime(dr["START_DATE"]),
                                   CLOSE_DATE = Convert.ToDateTime(dr["CLOSE_DATE"]),
                               }).ToList();
            return PartialView("_closedProjectList", projectList as List<projectList>);
        }
        [NonAction]
       
        public string getCoordinatrDetailCmd(string IIRNO)
        {
            string Command = "SELECT [IIRNO],[NAME],[DEPT],[DESIG] FROM [CO_NME] WHERE";
            //string Whereclause = string.Empty;
            bool AddAND = false;
            if (!string.IsNullOrEmpty(IIRNO))
            {
                //Whereclause = string.Format("APRLNO LIKE %{0}%", APRLNO);
                if (!AddAND)
                {
                    Command = Command + " " + string.Format("IIRNO LIKE '%{0}%'", IIRNO);
                    AddAND = true;
                }
            }


            return Command;
        }
        [NonAction]
        public string getCommandMstlst(string IIRNO)
        {
            string Command = "SELECT  [NPRNO],[START_DATE],[CLOSE_DATE] FROM [MSTLST] WHERE ";

            if (!string.IsNullOrEmpty(IIRNO))
            {
                //Whereclause = string.Format("APRLNO LIKE %{0}%", APRLNO);

                Command = Command + " " + string.Format("[INSTID] LIKE '%{0}%' ", IIRNO);


            }
            return Command;
        }
    }
}