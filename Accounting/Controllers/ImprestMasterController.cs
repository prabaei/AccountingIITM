
using Accounting.data.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Accounting.Models;
using Accounting.data.Database.FoxOffice;
using Accounting.data.Database.PCFACCT;
using System.Data;
using System.Data.SqlClient;
using Accounting.data.Database.FACCT;
using PagedList;
using System.Data.Entity.Infrastructure;

namespace Accounting.Controllers
{
    
    public class ImprestMasterController : Controller
    {
        #region fields
        
        private readonly IfoxOfficeService _Ifoxofficeservice;
        private readonly IFacctService ifactservice;
        private readonly IAccountingDbModel iaccountdb;
        //private List<ImprestMasterModel> imprestlist;
        #endregion

        #region constructor
        public ImprestMasterController( IfoxOfficeService ifoxofficeservice, IFacctService _ifacctservice, IAccountingDbModel _iaccountdb)
        {
            
            this._Ifoxofficeservice = ifoxofficeservice;
            this.ifactservice = _ifacctservice;
            iaccountdb = _iaccountdb;
            ifactservice.openconnection();
            _Ifoxofficeservice.openconnection();
        }
        #endregion
        // GET: ImprestMaster
        #region indexregion
        public ActionResult Index()
        {
            ImprestMdl imprestmastermodel = new ImprestMdl()
            {
                //Imprestlist = new List<ImprestCordinatorDetails>(),
                cordmodel = new coordmodel() { cordDetails = new ImprestCordinatorDetails() },
                imprestprojectdetails = new ImprestProjectDetails(),
                projectmodel = new ProjectMdl() { ProjectList = new List<ImprestProjectDetails>() },
                paymentmodel = new PaymentMdl() { paymentList = new List<paymentDetails>() },
                paymentdetails = new paymentDetails(),
                accountdetail = new AccountMdl()
            };
            TempData["ImprestMdl"] = imprestmastermodel;
            //hide payment detail list 
            ViewData["paylist"] = "none";
            //hide coordinator list
            ViewData["coordlist"] = "none";
            //hide project detail list
            ViewData["projectList"] = "none";

            ViewData["projectinput"] = true;
            ViewData["enablePaymentInput"] = true;

            return View(TempData["ImprestMdl"] as ImprestMdl);
        }
        #endregion

        #region coordionator region
        public ActionResult Serarch(int? page = null)
        {
            var tempmdl = TempData["ImprestMdl"] as ImprestMdl;
            TempData["ImprestMdl"] = tempmdl;
            ViewData["pageSize"] = 7;
            ViewData["pageNumber"] = (page ?? 1);
            //hide payment detail list 
            ViewData["paylist"] = "none";
            //hide coordinator list
            ViewData["coordlist"] = "normal";
            //hide project detail list
            ViewData["projectList"] = "none";

            /*input enable disable control*/
            ViewData["projectinput"] = true;
            ViewData["enablePaymentInput"] = true;

            return View("Index", TempData["ImprestMdl"] as ImprestMdl);
        }
        [HttpPost]
        public ActionResult Serarch(coordmodel imprestmdl)
        {
            ViewData["pageSize"] = 7;
            ViewData["pageNumber"] = 1;
            var tempmdl = TempData["ImprestMdl"] as ImprestMdl;
            if (ModelState.IsValid)
            {
                tempmdl.cordmodel.cordDetails = imprestmdl.cordDetails;
                if (string.IsNullOrEmpty(imprestmdl.cordDetails.IIRNO))
                    return RedirectToAction("Index");
                DataTable project = new DataTable();
                string cmd = getCommandPmaster(imprestmdl.cordDetails.IIRNO);
                SqlDataReader reader = _Ifoxofficeservice.getmasterdetailbycmd(cmd);
                project.Load(reader);
                // project = reader.GetSchemaTable();
                List<ImprestCordinatorDetails> imprestlist = (from DataRow dr in project.Rows
                                                              select new ImprestCordinatorDetails()
                                                              {
                                                                  IIRNO = Convert.ToString(dr["IIRNO"].ToString()),
                                                                  Name = Convert.ToString(dr["NAME"].ToString()),
                                                                  Department = Convert.ToString(dr["DEPT"].ToString()),

                                                              }).ToList();

                if (imprestlist != null)
                {
                    if (imprestlist.Count == 1)
                    {
                        tempmdl.cordmodel.cordDetails = imprestmdl.cordDetails;
                        tempmdl.paymentdetails = new paymentDetails();
                        TempData["ImprestMdl"] = tempmdl;
                        //hide payment detail list 
                        ViewData["paylist"] = "none";
                        //hide coordinator list
                        ViewData["coordlist"] = "none";
                        //hide project detail list
                        ViewData["projectList"] = "none";

                        /*input enable disable control*/
                        ViewData["projectinput"] = true;
                        ViewData["enablePaymentInput"] = true;

                        return RedirectToAction("Select", new { IIRNO = imprestlist.Where(m => m.IIRNO.Trim() == imprestmdl.cordDetails.IIRNO.Trim()).Select(m => m.IIRNO).FirstOrDefault() });

                    }
                }
                tempmdl.cordmodel.Imprestlist = imprestlist;
                tempmdl.cordmodel.cordDetails = new ImprestCordinatorDetails();
                tempmdl.paymentdetails = new paymentDetails();
                TempData["ImprestMdl"] = tempmdl;
                //hide payment detail list 
                ViewData["paylist"] = "none";
                //hide coordinator list
                ViewData["coordlist"] = "normal";
                //hide project detail list
                ViewData["projectList"] = "none";

                /*input enable disable control*/
                ViewData["projectinput"] = true;
                ViewData["enablePaymentInput"] = true;

                return View("Index", new ImprestMdl()
                {
                    projectmodel = new ProjectMdl() { ProjectList = new List<ImprestProjectDetails>() },
                    cordmodel = new coordmodel()
                    {
                        Imprestlist = imprestlist,
                        cordDetails = new ImprestCordinatorDetails() {
                            IIRNO = imprestmdl.cordDetails.IIRNO
                        }
                    }/*, projectDetailsList = new List<ImprestProjectDetails>()*/,
                    imprestprojectdetails = new ImprestProjectDetails(),
                    paymentmodel = new PaymentMdl(),
                    paymentdetails = new paymentDetails(),
                    accountdetail = new AccountMdl()
                });
            }
            else
            {
                tempmdl.cordmodel.cordDetails = imprestmdl.cordDetails;
                TempData["ImprestMdl"] = tempmdl;

                //hide payment detail list 
                ViewData["paylist"] = "none";
                //hide coordinator list
                ViewData["coordlist"] = "none";
                //hide project detail list
                ViewData["projectList"] = "none";

                /*input enable disable control*/
                ViewData["projectinput"] = true;
                ViewData["enablePaymentInput"] = true;

                return View("Index", TempData["ImprestMdl"] as ImprestMdl);
            }

        }

        public ActionResult Select(string IIRNO)

        {
            var tempmdl = TempData["ImprestMdl"] as ImprestMdl;
            if (string.IsNullOrEmpty(IIRNO))
                return RedirectToAction("Index");
            DataTable project = new DataTable();

            string cmd = getCommandPmaster(IIRNO);
            SqlDataReader reader = _Ifoxofficeservice.getmasterdetailbycmd(cmd);
            project.Load(reader);
            // project = reader.GetSchemaTable();
            List<ImprestCordinatorDetails> imprestlist = (from DataRow dr in project.Rows
                                                          select new ImprestCordinatorDetails()
                                                          {
                                                              IIRNO = Convert.ToString(dr["IIRNO"].ToString()),
                                                              Name = Convert.ToString(dr["NAME"].ToString()),
                                                              Department = Convert.ToString(dr["DEPT"].ToString()),

                                                          }).ToList();
            tempmdl.cordmodel.cordDetails = imprestlist.FirstOrDefault();
            //hide payment detail list 
            ViewData["paylist"] = "none";
            //hide coordinator list
            ViewData["coordlist"] = "none";
            //hide project detail list
            ViewData["projectList"] = "none";
            tempmdl.projectmodel = new ProjectMdl() { ProjectList = new List<ImprestProjectDetails>() };
            tempmdl.imprestprojectdetails = new ImprestProjectDetails();
            TempData["ImprestMdl"] = tempmdl; /*new ImprestMdl() { cordDetails = imprestlist.FirstOrDefault(), Imprestlist = new List<ImprestCordinatorDetails>(), projectDetailsList = new List<ImprestProjectDetails>(), imprestprojectdetails = new ImprestProjectDetails(), paymentdetails = new List<paymentDetails>(),projectmodel=new ProjectMdl() };*/


            /*input enable disable control*/
            ViewData["projectinput"] = false;
            ViewData["enablePaymentInput"] = true;

            return View("Index", TempData["ImprestMdl"] as ImprestMdl);
        }
        #endregion

        #region project region
        public ActionResult projectsearch(int? page = null)
        {
            var tempmdl = TempData["ImprestMdl"] as ImprestMdl;
            TempData["ImprestMdl"] = tempmdl;
            ViewData["pageSize"] = 7;
            ViewData["pageNumber"] = (page ?? 1);
            //hide payment detail list 
            ViewData["paylist"] = "none";
            //hide coordinator list
            ViewData["coordlist"] = "none";
            //hide project detail list
            ViewData["projectList"] = "normal";

            /*input enable disable control*/
            ViewData["projectinput"] = true;
            ViewData["enablePaymentInput"] = true;

            return View("Index", TempData["ImprestMdl"] as ImprestMdl);
        }
        [HttpPost]
        public ActionResult projectsearch(ProjectMdl mdl)
        {
            var tempmdl = TempData["ImprestMdl"] as ImprestMdl;
            DataTable project = new DataTable();
            ViewData["pageSize"] = 7;
            ViewData["pageNumber"] = 1;
            if (ModelState.IsValid)
            {


                string cmd = getCommandMstlst(tempmdl.cordmodel.cordDetails.IIRNO, mdl.projectNo);
                SqlDataReader reader = ifactservice.FetchfromFacc(cmd);
                project.Load(reader);
                
                List<ImprestProjectDetails> projectlist = (from DataRow dr in project.Rows

                                                           select new ImprestProjectDetails()
                                                           {
                                                               ProjectNo = Convert.ToString(dr["NPRNO"].ToString()),
                                                               ProjectStartDate = Convert.ToDateTime(dr["START_DATE"]),
                                                               ProjectCloseDate = Convert.ToDateTime(dr["CLOSE_DATE"])
                                                           }).ToList();

                if (projectlist.Count == 1)
                {
                    tempmdl.imprestprojectdetails = projectlist.FirstOrDefault();
                    tempmdl.paymentdetails = new paymentDetails();
                    TempData["ImprestMdl"] = tempmdl;

                    //hide payment detail list 
                    ViewData["paylist"] = "none";
                    //hide coordinator list
                    ViewData["coordlist"] = "none";
                    //hide project detail list
                    ViewData["projectList"] = "none";

                    /*input enable disable control*/
                    ViewData["projectinput"] = false;
                    ViewData["enablePaymentInput"] = false;

                    return View("Index", TempData["ImprestMdl"] as ImprestMdl);
                }
                else
                {
                    tempmdl.projectmodel.ProjectList = projectlist;
                    tempmdl.imprestprojectdetails = new ImprestProjectDetails();
                    tempmdl.paymentdetails = new paymentDetails();
                    TempData["ImprestMdl"] = tempmdl;

                    //hide payment detail list 
                    ViewData["paylist"] = "none";
                    //hide coordinator list
                    ViewData["coordlist"] = "none";
                    //hide project detail list
                    ViewData["projectList"] = "normal";

                    /*input enable disable control*/
                    ViewData["projectinput"] = false;
                    ViewData["enablePaymentInput"] = true;


                    return View("Index", TempData["ImprestMdl"] as ImprestMdl);

                }



            }
            else
            {
                tempmdl.projectmodel = mdl;
                TempData["ImprestMdl"] = tempmdl;

                //hide payment detail list 
                ViewData["paylist"] = "none";
                //hide coordinator list
                ViewData["coordlist"] = "none";
                //hide project detail list
                ViewData["projectList"] = "normal";

                /*input enable disable control*/
                ViewData["projectinput"] = false;
                ViewData["enablePaymentInput"] = true;

                return View("Index", TempData["ImprestMdl"] as ImprestMdl);
            }


        }
        public ActionResult projectSelect(string projectno = null)
        {

            var imprestmdl = TempData["ImprestMdl"] as ImprestMdl;
            imprestmdl.imprestprojectdetails = imprestmdl.projectmodel.ProjectList.Where(m => m.ProjectNo.Trim() == projectno.Trim()).FirstOrDefault();
            imprestmdl.paymentmodel = new PaymentMdl();
            //imprestmdl.projectDetailsList = new List<ImprestProjectDetails>();
            TempData["ImprestMdl"] = imprestmdl;


            //hide payment detail list 
            ViewData["paylist"] = "none";
            //hide coordinator list
            ViewData["coordlist"] = "none";
            //hide project detail list
            ViewData["projectList"] = "none";

            /*input enable disable control*/
            ViewData["projectinput"] = false;
            ViewData["enablePaymentInput"] = false;

            return View("Index", TempData["ImprestMdl"] as ImprestMdl);
        }
        #endregion

        #region payment region
        public ActionResult PaymentDetails(int? page = null)
        {
            var tempmdl = TempData["ImprestMdl"] as ImprestMdl;
            TempData["ImprestMdl"] = tempmdl;
            ViewData["pageSize"] = 7;
            ViewData["pageNumber"] = (page ?? 1);
            //hide payment detail list 
            ViewData["paylist"] = "normal";
            //hide coordinator list
            ViewData["coordlist"] = "none";
            //hide project detail list
            ViewData["projectList"] = "none";

            /*input enable disable control*/
            ViewData["projectinput"] = false;
            ViewData["enablePaymentInput"] = false;

            return View("Index", TempData["ImprestMdl"] as ImprestMdl);
        }

        [HttpPost]
        public ActionResult PaymentDetails(PaymentMdl paymdl)
        {
            var imprestmdl = TempData["ImprestMdl"] as ImprestMdl;
            ViewData["pageSize"] = 7;
            ViewData["pageNumber"] = 1;
            if (ModelState.IsValid)
            {
                imprestmdl.paymentmodel.paymentList =
                getProjectVoucherDetails(imprestmdl.imprestprojectdetails.ProjectStartDate, imprestmdl.imprestprojectdetails.ProjectNo, paymdl.VoucherNo);

                if (imprestmdl.paymentmodel.paymentList.Count == 1)
                {
                    imprestmdl.paymentdetails = imprestmdl.paymentmodel.paymentList.FirstOrDefault();
                    TempData["ImprestMdl"] = imprestmdl;
                    //hide payment detail list 
                    ViewData["paylist"] = "none";
                    //hide coordinator list
                    ViewData["coordlist"] = "none";
                    //hide project detail list
                    ViewData["projectList"] = "none";

                    /*input enable disable control*/
                    ViewData["projectinput"] = false;
                    ViewData["enablePaymentInput"] = false;

                    

                    return View("Index", TempData["ImprestMdl"] as ImprestMdl);
                }
                TempData["ImprestMdl"] = imprestmdl;
                //hide payment detail list 
                ViewData["paylist"] = "normal";
                //hide coordinator list
                ViewData["coordlist"] = "none";
                //hide project detail list
                ViewData["projectList"] = "none";

                /*input enable disable control*/
                ViewData["projectinput"] = false;
                ViewData["enablePaymentInput"] = false;

                return View("Index", TempData["ImprestMdl"] as ImprestMdl);
            }
            else
            {
                imprestmdl.paymentmodel = paymdl;
                TempData["ImprestMdl"] = imprestmdl;
                //hide payment detail list 
                ViewData["paylist"] = "normal";
                //hide coordinator list
                ViewData["coordlist"] = "none";
                //hide project detail list
                ViewData["projectList"] = "none";

                /*input enable disable control*/
                ViewData["projectinput"] = false;
                ViewData["enablePaymentInput"] = false;

                return View("Index", TempData["ImprestMdl"] as ImprestMdl);
            }
        }

        public ActionResult VoucherSelect(string voucherno = null)
        {
            var imprestmdl = TempData["ImprestMdl"] as ImprestMdl;

            imprestmdl.paymentdetails.AmountPayed = imprestmdl.paymentmodel.paymentList.Where(m => m.VoucherNo == voucherno).Select(m => m.AmountPayed).FirstOrDefault();
            imprestmdl.paymentdetails.dateofDrawn = imprestmdl.paymentmodel.paymentList.Where(m => m.VoucherNo == voucherno).Select(m => m.dateofDrawn).FirstOrDefault();
            imprestmdl.paymentdetails.VoucherNo = voucherno;

            TempData["ImprestMdl"] = imprestmdl;

            //hide payment detail list 
            ViewData["paylist"] = "none";
            //hide coordinator list
            ViewData["coordlist"] = "none";
            //hide project detail list
            ViewData["projectList"] = "none";

            /*input enable disable control*/
            ViewData["projectinput"] = false;
            ViewData["enablePaymentInput"] = false;
            
            
            ImprestMdl model = TempData["ImprestMdl"] as ImprestMdl;
            return View("Index", model);
        }

        #endregion

        #region accoutn region
        [HttpPost]
        public ActionResult AccountDetails(ImprestMdl mdl)
        {
            ViewData["projectinput"] = false;
            ViewData["enablePaymentInput"] = false;
            if (ModelState.IsValid)

            {
                if (string.Equals("ok", mdl.accountdetail.conf))
                {
                    ImprestMaster master = new ImprestMaster()
                    {
                        //AmountPaid = Convert.ToDecimal(mdl.paymentdetails.AmountPayed),
                        //BankAccountNumber = mdl.accountdetail.BankAccountNumber,
                        //CoorName = mdl.cordmodel.cordDetails.Name,
                        //InstituteId = mdl.cordmodel.cordDetails.IIRNO,
                        //Limit = mdl.accountdetail.Limit,
                        //DateOfDrawn = mdl.paymentdetails.dateofDrawn,
                        //VoucherNo = mdl.paymentdetails.VoucherNo,
                        //ProjectStartDate = mdl.imprestprojectdetails.ProjectStartDate,
                        //ProjectEndDate = mdl.imprestprojectdetails.ProjectCloseDate,
                        //Department = mdl.cordmodel.cordDetails.Department,
                        //ProjectNo = mdl.imprestprojectdetails.ProjectNo

                    };
                    iaccountdb.ImprestMasters.Add(master);
                    try
                    {
                        if (iaccountdb.SaveChanges() == 1)
                        {
                            ModelState.Clear();
                            // mdl.accountdetail.conf = "saved";
                            //hide payment detail list 
                            ViewData["paylist"] = "none";
                            //hide coordinator list
                            ViewData["coordlist"] = "none";
                            //hide project detail list
                            ViewData["projectList"] = "none";

                            ViewData["projectinput"] = true;
                            ViewData["enablePaymentInput"] = true;
                            ImprestMdl imprestmastermodel = new ImprestMdl()
                            {
                                //Imprestlist = new List<ImprestCordinatorDetails>(),
                                cordmodel = new coordmodel() { cordDetails = new ImprestCordinatorDetails() },
                                imprestprojectdetails = new ImprestProjectDetails(),
                                projectmodel = new ProjectMdl() { ProjectList = new List<ImprestProjectDetails>() },
                                paymentmodel = new PaymentMdl() { paymentList = new List<paymentDetails>() },
                                paymentdetails = new paymentDetails(),
                                accountdetail = new AccountMdl()
                            };
                            imprestmastermodel.accountdetail.conf = "saved";
                            ViewData["alertclass"] = "alert-success";
                            ViewData["alertmsg"] = "Details saved";
                            return View("Index", imprestmastermodel);
                        }
                    }
                    catch (DbUpdateException ex)
                    {
                        var sqlex = ex.InnerException.InnerException as SqlException;
                        if (sqlex != null)
                        {
                            switch (sqlex.Number)
                            {
                                case 2627:
                                    ModelState.Clear();
                                    // mdl.accountdetail.conf = "saved";
                                    //hide payment detail list 
                                    ViewData["paylist"] = "none";
                                    //hide coordinator list
                                    ViewData["coordlist"] = "none";
                                    //hide project detail list
                                    ViewData["projectList"] = "none";

                                    ViewData["projectinput"] = true;
                                    ViewData["enablePaymentInput"] = true;
                                    ImprestMdl imprestmastermodel = new ImprestMdl()
                                    {
                                        //Imprestlist = new List<ImprestCordinatorDetails>(),
                                        cordmodel = new coordmodel() { cordDetails = new ImprestCordinatorDetails() },
                                        imprestprojectdetails = new ImprestProjectDetails(),
                                        projectmodel = new ProjectMdl() { ProjectList = new List<ImprestProjectDetails>() },
                                        paymentmodel = new PaymentMdl() { paymentList = new List<paymentDetails>() },
                                        paymentdetails = new paymentDetails(),
                                        accountdetail = new AccountMdl()
                                    };
                                    imprestmastermodel.accountdetail.conf = "duplicate";
                                    ViewData["alertclass"] = "alert-danger";
                                    ViewData["alertmsg"] = "Duplicate record save discarded";
                                    return View("Index", imprestmastermodel);
                                    //break;
                            }
                        }
                        throw ex;
                    }


                }
                ModelState.Clear();
                /*input enable disable control*/
                ViewData["projectinput"] = true;
                ViewData["enablePaymentInput"] = true;
                mdl.accountdetail.conf = "ok";
                return View("Index", mdl);
            }
            else
            {
                mdl.projectmodel = new ProjectMdl();
                mdl.paymentmodel = new PaymentMdl();
                //mdl.cordmodel = new coordmodel();
                ViewData["pageSize"] = 7;
                ViewData["pageNumber"] = 1;

                /*input enable disable control*/
                ViewData["projectinput"] = true;
                ViewData["enablePaymentInput"] = true;

                return View("Index", mdl);
            }

        }
        #endregion

        #region non action region
        //[NonAction]
        //public string getCommandPmaster(string IIRNO, string Name, string Department)
        //{
        //    string Command = "SELECT [IIRNO],[NAME],[DEPT] FROM [CO_NME] WHERE";
        //    //string Whereclause = string.Empty;
        //    bool AddAND = false;
        //    if (!string.IsNullOrEmpty(IIRNO))
        //    {
        //        //Whereclause = string.Format("APRLNO LIKE %{0}%", APRLNO);
        //        if (!AddAND)
        //        {
        //            Command = Command + " " + string.Format("IIRNO LIKE '%{0}%'", IIRNO);
        //            AddAND = true;
        //        }
        //    }
        //    // Whereclause = string.Format("APRLNO LIKE %{0}%", APRLNO);
        //    if (!string.IsNullOrEmpty(Name))
        //    {
        //        if (!AddAND)
        //        {
        //            Command = Command + " " + string.Format("[NAME] LIKE '%{0}%'", Name);
        //            AddAND = true;
        //        }
        //        else
        //        {
        //            Command = Command + " " + string.Format("{1} [NAME] LIKE '%{0}%'", Name, "AND");
        //        }
        //    }
        //    // Whereclause = string.Format("[COOR_NAME] LIKE %{0}%", coordinatorname);
        //    if (!string.IsNullOrEmpty(Department))
        //    {
        //        if (!AddAND)
        //        {
        //            Command = Command + " " + string.Format("[DEPT] = '{0}'", Department);
        //            AddAND = true;
        //        }
        //        else
        //        {
        //            Command = Command + " " + string.Format("{1} [DEPT] = '%{0}%'", Department, "AND");
        //        }
        //    }

        //    return Command;
        //}
        [NonAction]
        public string getCommandPmaster(string IIRNO)
        {
            string Command = "SELECT [IIRNO],[NAME],[DEPT] FROM [CO_NME] WHERE";
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
        public string getCommandMstlst(string IIRNO, string nprno)
        {
            string Command = "SELECT  [NPRNO],[START_DATE],[CLOSE_DATE] FROM [MSTLST] WHERE ";
            //string Whereclause = string.Empty;
            bool AddAND = false;
            if (!string.IsNullOrEmpty(IIRNO))
            {
                //Whereclause = string.Format("APRLNO LIKE %{0}%", APRLNO);
                if (!AddAND)
                {
                    Command = Command + " " + string.Format("[INSTID] like '%{0}%' AND [NPRNO] LIKE '%{1}%'", IIRNO, nprno);
                    AddAND = true;
                }
            }
            return Command;
        }
        [NonAction]
        public List<paymentDetails> getProjectVoucherDetails(DateTime? projectstartdate, string projectno, string voucherno)
        {
            //List<string> TableName = new List<string>();
            int projectStYear = projectstartdate.Value.Year % 100;
            bool tableExist = true;
            List<paymentDetails> paylist = new List<paymentDetails>();
            while (tableExist)
            {
                string yearinstring = Convert.ToString(projectStYear);

                DataTable paydetails = new DataTable();
                yearinstring = yearinstring + Convert.ToString(projectStYear + 1);

                if (ifactservice.checkTableExistence(string.Format("VOU{0}", yearinstring)) == 1)
                {
                    string cmd = string.Format("SELECT [VRNO],[DATE], SUM([AMOUNT]) AS AMOUNT FROM VOU{0} WHERE [NPRNO] = '{1}' GROUP BY [VRNO],[DATE],[NPRNO] HAVING [VRNO] LIKE '%{2}%' ", yearinstring, projectno, voucherno);
                    //string cmd = string.Format("SELECT VRNO, AMOUNT, DATE FROM VOU{0} WHERE NPRNO = '{1}' AND VRNO LIKE '%{2}%'", yearinstring, projectno, voucherno);
                    SqlDataReader reader = ifactservice.FetchfromFacc(cmd);
                    paydetails.Load(reader);
                    var paymentlist = (from DataRow dr in paydetails.Rows
                                       select new paymentDetails()
                                       {
                                           VoucherNo = Convert.ToString(dr["VRNO"].ToString()),
                                           AmountPayed = Convert.ToDouble(dr["AMOUNT"]),
                                           dateofDrawn = Convert.ToDateTime(dr["DATE"]),
                                       }).ToList();
                    paylist.AddRange(paymentlist as List<paymentDetails>);

                }
                else
                {
                    tableExist = false;
                }
                projectStYear++;
            }

            return paylist;
        }
        
        
        #endregion
    }
}