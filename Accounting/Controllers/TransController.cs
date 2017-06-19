using Accounting.AccountAuthorize;
using Accounting.data.Database;
using Accounting.data.Database.FACCT;
using Accounting.data.Database.PCFACCT;
using Accounting.data.ICSRTallyDb;
using Accounting.data.services.Export;
using Accounting.data.services.Logger;
using Accounting.Models.Trans;
using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Accounting.Controllers
{
    [AccountAutherize]
    public class TransController : Controller
    {
        readonly IPCFACCTService _ipcfactservice;
        readonly IFacctService _Facctservice;
        readonly IAccountingDbModel _accountdbmodel;
        private readonly IExportToExcel _IexporttoExcel;
        readonly IExceptionLogger _Iexlogger;
        private readonly IICSRDBTALLYEntities _icsrtally;
        public TransController(IPCFACCTService ipcfacctservice, IFacctService _facctservice, IAccountingDbModel AccountDb, IExportToExcel Export, IExceptionLogger Iexlogger, IICSRDBTALLYEntities Icsrtally)
        {
            _ipcfactservice = ipcfacctservice;
            _Facctservice = _facctservice;
            _accountdbmodel = AccountDb;
            _IexporttoExcel = Export;
            _Iexlogger = Iexlogger;
            _icsrtally = Icsrtally;
            _ipcfactservice.openconnection();
            _facctservice.openconnection();
            _icsrtally = Icsrtally;
        }


        //protected override void OnException(ExceptionContext filterContext)
        //{
        //    Exception ex = filterContext.Exception;
        //    filterContext.ExceptionHandled = true;

        //    var model = new HandleErrorInfo(filterContext.Exception, "Controller", "Action");

        //    filterContext.Result = new ViewResult()
        //    {
        //        ViewName = "Error",
        //        ViewData = new ViewDataDictionary(model)
        //    };

        //}


        // GET: Trans
        [AccountAutherize]
        public ActionResult Index(string TransNO = null)
        {

            //List<recentTrans> RecentHistory = _accountdbmodel.Transaction.Where(m => m.TransCommited == true && m.UserEmail == User.Identity.Name && m.deleted == false).OrderBy(m => m.TransCreated).Select().ToList();

            IEnumerable<recentTrans> RecentHistory = (from acc in _accountdbmodel.Transaction
                                                      where acc.TransCommited == true && acc.UserEmail == User.Identity.Name
                                                      join voutype in _accountdbmodel.VoucherTypes on acc.voucherType equals voutype.TypeId
                                                      orderby acc.TransCreated descending
                                                      select new recentTrans()
                                                      {
                                                          amount = acc.Amount,
                                                          Date = acc.bankDate,
                                                          VoucherType = voutype.VoucherTypeName
                                                      }).ToList().Take(5);

            //if (string.IsNullOrEmpty(vouchertype))
            //{

            if (!string.IsNullOrEmpty(TransNO))
            {
                Transaction trans = _accountdbmodel.Transaction.Where(m => m.TransNO == TransNO.Trim()).FirstOrDefault();
                trans.INSTID = _accountdbmodel.ImprestMasters.Where(m => m.BankAccountNo == trans.BankAccountNO.Trim()).Select(m => m.InstituteId).FirstOrDefault();
                trans.bnkdate = trans.bankDate != null ? Convert.ToDateTime(trans.bankDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "";
                trans.CoordinatorName = _accountdbmodel.ImprestMasters.Where(m => m.InstituteId == trans.INSTID.Trim()).Select(m => m.CoordinatorName).FirstOrDefault();
                trans.voucherTypeList = _accountdbmodel.VoucherTypes.Select(m => new SelectListItem() { Text = m.VoucherTypeName, Value = m.TypeId.ToString() }).ToList();
                trans.recentHistory = RecentHistory == null ? new List<recentTrans>() : RecentHistory;
                trans.supplierstr = _accountdbmodel.supplier.Where(m => m.Id == trans.supplier).Select(m => m.Name).FirstOrDefault();
                return View("IndexEdit", trans);
            }
            List<Transaction> uncommitedTrans = _accountdbmodel.Transaction.Where(m => m.TransCommited == false && m.UserEmail.Trim() == User.Identity.Name.Trim()).ToList();
            _accountdbmodel.Transaction.RemoveRange(uncommitedTrans);
            _accountdbmodel.SaveChanges();
            return View(new Transaction() { voucherType = 2, voucherTypeList = _accountdbmodel.VoucherTypes.Where(m => m.ForEntry == true).Select(m => new SelectListItem() { Text = m.VoucherTypeName, Value = m.TypeId.ToString() }).ToList(), recentHistory = RecentHistory == null ? new List<recentTrans>() : RecentHistory });
            //}
            //else
            //{
            //    switch (vouchertype)
            //    {
            //        case "1"://recoupement
            //            return View(new Transaction() { voucherType = 1 });

            //        case "2"://chequePayment
            //            return View(new Transaction() { voucherType = 2 });

            //        case "8"://ImprestSanction
            //            return View(new Transaction() { voucherType = 8 });

            //        default:
            //            return View(new Transaction());

            //    }
            //}

        }
        [HttpPost]
        public JsonResult INSTID(string searchStr)
        {

            var accoutlist = _accountdbmodel.ImprestMasters.Where(m => m.InstituteId.Contains(searchStr.Trim())).Select(m => new { instid = m.InstituteId, name = m.CoordinatorName, accno = m.BankAccountNo }).ToList().Take(5);
            //string cmd = string.Format("SELECT TOP 7 [NAME],[IIRNO] FROM [CO_NME] WHERE [IIRNO] LIKE '%{0}%'", searchStr);
            //SqlDataReader st = _ipcfactservice.getIPCFFACTdata(cmd);
            //DataTable dt = new DataTable();
            //dt.Load(st);
            //var IIROList = (from DataRow dr in dt.Rows
            //                select new
            //                {
            //                    INSTID = Convert.ToString(dr["IIRNO"].ToString()),
            //                    Name = dr["NAME"].ToString(),

            //                }).ToList();
            return Json(new { message = "success", iirolist = accoutlist }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ProjectNO(string instid, string projectno)
        {
            if (!string.IsNullOrEmpty(instid))
            {
                string Command = "SELECT TOP 7  [NPRNO],1 as [PT],[TITLE],[COOR_NAME],[START_DATE],[CLOSE_DATE],[INSTID] FROM [MSTLST] WHERE";
                Command = Command + " " + string.Format("[INSTID] like '%{0}%' AND [NPRNO] LIKE '%{1}%' ", instid, projectno);
                //SqlDataReader st = new SqlDataReader()
                SqlDataReader st = _Facctservice.FetchfromFacc(Command);
                DataTable dt = new DataTable();
                dt.Load(st);
                var projectList = (from DataRow datro in dt.Rows
                                   select new
                                   {
                                       ProjectNo = Convert.ToString(datro["NPRNO"]),
                                       TITLE = Convert.ToString(datro["TITLE"]),
                                       STARTDATE = DBNull.Value.Equals(datro["START_DATE"]) == true ? "" : Convert.ToDateTime(datro["START_DATE"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),//Convert.ToString(datro["S_DATE"]) == null ? "" : Convert.ToDateTime(datro["S_DATE"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                                       CLOSEDATE = DBNull.Value.Equals(datro["CLOSE_DATE"]) == true ? "" : Convert.ToDateTime(datro["CLOSE_DATE"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                                       NAME = Convert.ToString(datro["COOR_NAME"]),
                                       TYPE = Convert.ToString(datro["PT"]),
                                   }).ToList();
                Command = "SELECT [CPRNO],2 as [PT],[AGENCY],[C_TITLE],[S_DATE],[C_DATE],[COOR_NAME1],[DEPT] FROM [CMSTLST] ";
                Command = Command + " " + string.Format("WHERE INSTID LIKE '%{0}%' AND [CPRNO] LIKE '%{1}%'", instid, projectno);
                SqlDataReader sst = _Facctservice.FetchfromFacc(Command);
                DataTable ddt = new DataTable();
                ddt.Load(sst);
                var consultProjectList = (from DataRow datro in ddt.Rows
                                          select new
                                          {
                                              ProjectNo = Convert.ToString(datro["CPRNO"]),
                                              TITLE = Convert.ToString(datro["C_TITLE"]),
                                              STARTDATE = DBNull.Value.Equals(datro["S_DATE"]) == true ? "" : Convert.ToDateTime(datro["S_DATE"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),//Convert.ToString(datro["S_DATE"]) == null ? "" : Convert.ToDateTime(datro["S_DATE"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                                              CLOSEDATE = DBNull.Value.Equals(datro["C_DATE"]) == true ? "" : Convert.ToDateTime(datro["C_DATE"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                                              NAME = Convert.ToString(datro["COOR_NAME1"]),
                                              TYPE = Convert.ToString(datro["PT"]),
                                          }).ToList();
                projectList.AddRange(consultProjectList);
                return Json(new { message = "success", projectlist = projectList.Take(5) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { message = "InstidNotSelected", projectlist = "" }, JsonRequestBehavior.AllowGet);
            }


        }

        //[HttpPost]
        //public JsonResult ProjectNOSelect(string instid, string projectno)
        //{
        //    string cmd = string.Format("SELECT COUNT(CPRNO) AS TCOUNT FROM [CMSTLST] WHERE [INSTID] = '{0}' and CPRNO = '{1}'", instid, projectno);
        //    if (rowcount(cmd) == 1)
        //    {
        //        cmd = string.Format("SELECT [CPRNO],[AGENCY],[C_TITLE],[S_DATE],[C_DATE],[COOR_NAME1],[DEPT] FROM [CMSTLST] WHERE [INSTID] ='{0}' AND CPRNO ='{1}'", instid, projectno);
        //        SqlDataReader sst = _Facctservice.FetchfromFacc(cmd);
        //        DataTable ddt = new DataTable();
        //        ddt.Load(sst);
        //        var consultProjectList = (from DataRow datro in ddt.Rows
        //                                  select new
        //                                  {
        //                                      ProjectNo = Convert.ToString(datro["CPRNO"]),
        //                                      TITLE = Convert.ToString(datro["C_TITLE"]),
        //                                      STARTDATE = DBNull.Value.Equals(datro["S_DATE"]) == true ? "" : Convert.ToDateTime(datro["S_DATE"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),//Convert.ToString(datro["S_DATE"]) == null ? "" : Convert.ToDateTime(datro["S_DATE"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
        //                                      CLOSEDATE = DBNull.Value.Equals(datro["C_DATE"]) == true ? "" : Convert.ToDateTime(datro["C_DATE"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
        //                                      NAME = Convert.ToString(datro["COOR_NAME1"]),
        //                                      TYPE = "CMSTLST"
        //                                  }).FirstOrDefault();

        //        return Json(new { job = "s", project = consultProjectList }, JsonRequestBehavior.AllowGet);

        //    }
        //    cmd = string.Format("SELECT count(NPRNO) AS TCOUNT FROM [FACCT].[dbo].[MSTLST] WHERE INSTID='{0}' AND NPRNO='{1}'", instid, projectno);
        //    if (rowcount(cmd) == 1)
        //    {
        //        cmd = string.Format("SELECT  [NPRNO],[TITLE],[COOR_NAME],[START_DATE],[CLOSE_DATE],[INSTID] FROM [MSTLST] WHERE INSTID='{0}' AND NPRNO='{1}'", instid, projectno);
        //        SqlDataReader sst = _Facctservice.FetchfromFacc(cmd);
        //        DataTable ddt = new DataTable();
        //        ddt.Clear();
        //        ddt.Load(sst);
        //        var consultProjectList = (from DataRow datro in ddt.Rows
        //                                  select new
        //                                  {
        //                                      ProjectNo = Convert.ToString(datro["NPRNO"]),
        //                                      TITLE = Convert.ToString(datro["TITLE"]),
        //                                      STARTDATE = DBNull.Value.Equals(datro["START_DATE"]) == true ? "" : Convert.ToDateTime(datro["START_DATE"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),//Convert.ToString(datro["S_DATE"]) == null ? "" : Convert.ToDateTime(datro["S_DATE"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
        //                                      CLOSEDATE = DBNull.Value.Equals(datro["CLOSE_DATE"]) == true ? "" : Convert.ToDateTime(datro["CLOSE_DATE"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
        //                                      NAME = Convert.ToString(datro["COOR_NAME"]),
        //                                      TYPE = "MSTLST"
        //                                  }).FirstOrDefault();

        //        return Json(new { job = "s", project = consultProjectList }, JsonRequestBehavior.AllowGet);
        //    }
        //    return Json(new { job = "f" }, JsonRequestBehavior.AllowGet);
        //}

        //[NonAction]
        //public int rowcount(string cmd)
        //{
        //    SqlDataReader sst = _Facctservice.FetchfromFacc(cmd);
        //    DataTable ddt = new DataTable();
        //    ddt.Load(sst);
        //    DataRow datro = ddt.Rows[0];
        //    return Convert.ToInt16(datro["TCOUNT"]);

        //}
        [HttpPost]
        public JsonResult coorname(string coordinatorName)
        {
            var coorname = _accountdbmodel.ImprestMasters.Where(m => m.CoordinatorName.Contains(coordinatorName.Trim())).Select(m => new { name = m.CoordinatorName, accno = m.BankAccountNo, instid = m.InstituteId }).ToList().Take(5);
            return Json(new { job = 's', coordName = coorname }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AccountNO(string AccountNO)
        {
            var accountnolist = _accountdbmodel.ImprestMasters.Where(m => m.BankAccountNo.Contains(AccountNO)).Select(m => new { name = m.CoordinatorName, accno = m.BankAccountNo, instid = m.InstituteId }).ToList().Take(5);
            //_accountdbmodel.ImprestMasters.Where(m => m.BankAccountNo.Contains(AccountNO)).Select(m => new { accno = m.BankAccountNo });
            return Json(new { job = 's', accnolst = accountnolist }, JsonRequestBehavior.AllowGet);

        }

        //[HttpPost]
        //public JsonResult InstituteId(string acno)
        //{
        //    var instituteId = _accountdbmodel.ImprestMasters.Where(m => m.BankAccountNo == acno).Select(m => new { instid = m.InstituteId, name = m.CoordinatorName, accno = m.BankAccountNo }).FirstOrDefault();
        //    //_accountdbmodel.ImprestMasters.Where(m => m.BankAccountNo.Contains(AccountNO)).Select(m => new { accno = m.BankAccountNo });
        //    return Json(new { job = 's', instid = instituteId.instid }, JsonRequestBehavior.AllowGet);

        //}
        [HttpPost]
        public JsonResult ConsignmentNO(string searchStr, string projectStDate, string projectNO, string type, string filterbool, string filterstr)
        {

            if (string.IsNullOrEmpty(projectNO))
            {
                return Json(new { job = "fP", ermsg = "ProjectNO not Selected" }, JsonRequestBehavior.AllowGet);
            }

            if (!string.IsNullOrEmpty(projectStDate))
            {
                DateTime startdate = DateTime.ParseExact(projectStDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //string CommitmentNO = string.Empty;
                //var commitmentlist = Enumerable.Empty<object>().Select(r => new { CommitmentNO="" }).ToList();
                var commitmentlist = getProjectConsignmentDetails(startdate, projectNO, searchStr, type, filterbool, filterstr);
                if (commitmentlist != null)
                    return Json(new { job = 's', comList = commitmentlist }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { job = "el" }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                /// doesnot have start date..
            }

            return null;
        }

        [HttpGet]
        public ActionResult VoucherDetails(string searchStr, string filterbool, string filterstr, string instid, string Type, string edit = null)
        {
            DateTime dt = DateTime.Now.AddYears(-2);
            var getvoucher = getProjectVoucher(dt, searchStr, Type, filterbool, filterstr, instid);
            if (!string.IsNullOrEmpty(edit))
            {
                return PartialView("VoucherDetailsed", (List<VouDetail>)getvoucher);
            }
            return PartialView("VoucherDetails", (List<VouDetail>)getvoucher);
            //return Json(new { job = "s", errMsg = "", data = da }, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public JsonResult VoucherNO(string searchStr, string projectNO, string commitmentNo, string projectStDate, string type, string filterbool, string filterstr)
        //{
        //    if (string.IsNullOrEmpty(projectNO))
        //    {
        //        return Json(new { job = "fP", ermsg = "ProjectNO not Selected" }, JsonRequestBehavior.AllowGet);
        //    }
        //    //if (string.IsNullOrEmpty(commitmentNo))
        //    //{
        //    //    //return Json(new { job = "fC", ermsg = "Commitment not Selected" }, JsonRequestBehavior.AllowGet);


        //    //}

        //    if (!string.IsNullOrEmpty(projectStDate))
        //    {
        //        DateTime startdate = DateTime.ParseExact(projectStDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        //        var voucherLst = new object();
        //        try
        //        {
        //            voucherLst = getProjectVoucher(startdate, projectNO, commitmentNo, searchStr, type, filterbool, filterstr);

        //        }
        //        catch
        //        {
        //            return Json(new { job = "el" }, JsonRequestBehavior.AllowGet);

        //        }

        //        //string CommitmentNO = string.Empty;
        //        //var commitmentlist = Enumerable.Empty<object>().Select(r => new { CommitmentNO="" }).ToList();
        //        return Json(new { job = 's', voulst = voucherLst }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        /// doesnot have start date..
        //    }

        //    return null;
        //}

        public JsonResult VoucherNoUp(string searchStr, string filterbool, string filterstr, string instid)
        {
            if (string.IsNullOrEmpty(instid))
            {
                return Json(new { job = "fc", errMsg = "Select Coordinator", data = new List<VouDetail>() }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                DateTime dt = DateTime.Now.AddYears(-2);
                var dd = getVoucherOnly(dt, instid, searchStr, filterbool, filterstr);
                return Json(new { job = "s", errMsg = "", data = dd }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult TransactionPost(Transaction transaction)
        {
            DateTime? Bankdate = null;
            DateTime? chequdt = null;
            //string[] datearray= transaction.bnkdate.Split('/');
            if (!string.IsNullOrEmpty(transaction.bnkdate))
            {
                Bankdate = DateTime.ParseExact(transaction.bnkdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            }
            if (!string.IsNullOrEmpty(transaction.Cheqdt))
            {
                chequdt = DateTime.ParseExact(transaction.Cheqdt, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            }
            // st.Day = Convert.ToInt32(datearray[0]); 
            //if (string.Equals(transaction.projtype, "CMSTLST"))
            //{
            //    transaction.ProjectType = 2;
            //}
            //if (string.Equals(transaction.projtype, "MSTLST"))
            //{
            //    transaction.ProjectType = 1;
            //}
            //decimal? AvailableBal;
            //decimal? currentBal;
            //string errorMsg;
            //bool transd = commitTran(transaction.TransNO, transaction.ProjectNo, transaction.voucherType, transaction.voucherNo, transaction.Amount, Bankdate, transaction.ChequeNO, transaction.narration, transaction.Remarks, transaction.CommitmentNO, transaction.BankAccountNO, transaction.ProjectType, transaction.Head, transaction.Description, out currentBal, out AvailableBal, out errorMsg);
            Transaction Transave = _accountdbmodel.Transaction.Where(m => m.TransNO == transaction.TransNO).FirstOrDefault();
            ImprestMaster Mstr = _accountdbmodel.ImprestMasters.Where(m => m.BankAccountNo == transaction.BankAccountNO).FirstOrDefault();

            if (Transave != null)
            {
                if (Transave.TransCommited != true)
                {
                    Transave.ProjectNo = transaction.ProjectNo;
                    Transave.voucherType = transaction.voucherType;
                    Transave.Amount = transaction.Amount;
                    Transave.bankDate = Bankdate;
                    Transave.ChequeDt = chequdt;
                    Transave.ProjectType = transaction.ProjectType;
                    Transave.ChequeNO = transaction.ChequeNO;
                    Transave.narration = transaction.narration;
                    Transave.Remarks = transaction.Remarks;
                    Transave.CommitmentNO = transaction.CommitmentNO;
                    Transave.BankAccountNO = transaction.BankAccountNO;
                    Transave.currentBal = 0;
                    Transave.AvailableBal = 0;
                    Transave.voucherNo = transaction.voucherNo;
                    Transave.projtype = transaction.projtype;
                    Transave.Head = transaction.Head;
                    Transave.Description = transaction.Description;
                    Transave.supplier = transaction.supplier;
                    Transave.TransCommited = true;
                    if (_accountdbmodel.VoucherTypes.Where(m => m.TypeId == transaction.voucherType).Select(m => m.deposit).FirstOrDefault())
                    {
                        Mstr.Amount += (decimal)transaction.Amount;
                    }
                    else
                    {
                        Mstr.Amount -= (decimal)transaction.Amount;
                    }
                    //_accountdbmodel.Transaction.Attach(Transave);
                    //_accountdbmodel.ImprestMasters.Attach(Mstr);
                    _accountdbmodel.SaveChanges();
                }
                else
                {
                    return Json(new
                    {
                        currentamount = 0,
                        avilableamount = 0,
                        errmsg = "ALCMT",
                        transdone = "False",
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new
                {
                    currentamount = 0,
                    avilableamount = 0,
                    errmsg = "NOTRNO",
                    transdone = "False",
                }, JsonRequestBehavior.AllowGet);
            }
            //int transCount = _accountdbmodel.Transaction.Where(m => m.bankDate == Bankdate && m.BankAccountNO == transaction.BankAccountNO.Trim() && m.TransNO != transaction.TransNO.Trim()).Count();
            //var tvou=_accountdbmodel.VoucherTypes.Where(m => m.TypeId == transaction.voucherType).FirstOrDefault();
            //if (tvou.recoup)
            //{
            //    ImprestMaster imp= _accountdbmodel.ImprestMasters.Where(m => m.BankAccountNo == transaction.BankAccountNO).FirstOrDefault();
            //    imp.Amount += (decimal)transaction.Amount;
            //    _accountdbmodel.SaveChanges();
            //}
            //else
            //{
            //    ImprestMaster imp = _accountdbmodel.ImprestMasters.Where(m => m.BankAccountNo == transaction.BankAccountNO).FirstOrDefault();
            //    imp.Amount-= (decimal)transaction.Amount;
            //    _accountdbmodel.SaveChanges();
            //}


            //if(transCount>0)
            //{
            //    int maxorder = (int)_accountdbmodel.Transaction.Where(m => m.bankDate == Bankdate && m.BankAccountNO == transaction.BankAccountNO.Trim() && m.TransNO != transaction.TransNO.Trim() && m.deleted==false).Max(m => m.orderId);
            //    Transaction tran = _accountdbmodel.Transaction.Where(m => m.TransNO == transaction.TransNO.Trim()).FirstOrDefault();
            //    tran.orderId = maxorder+1;
            //    _accountdbmodel.SaveChanges();
            //}
            //else
            //{
            //    Transaction tran = _accountdbmodel.Transaction.Where(m =>  m.TransNO == transaction.TransNO.Trim()).FirstOrDefault();
            //    tran.orderId = 1;
            //    _accountdbmodel.SaveChanges();

            //}
            return Json(new
            {
                currentamount = 0,
                avilableamount = 0,
                errmsg = "",
                transdone = "True",
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GenTransNo(string searchStr)
        {
            string errormsg = string.Empty;
            string TransactionNo = string.Empty;
            bool isfailed = createTran(searchStr, User.Identity.Name, out TransactionNo, out errormsg);

            return Json(new
            {
                Transno = TransactionNo,
                Isfailedobj = Convert.ToString(isfailed),
                ermsgo = errormsg
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AccountTrans()
        {
            //AccountTrans completeMdl = new data.Database.AccountTrans();
            //if (TempData["AccTran"] != null)
            //{
            //    completeMdl = (AccountTrans)TempData["AccTran"];
            //    //TempData["AccTran"] = completeMdl;
            //}

            IEnumerable<SelectListItem> vouchertype = _accountdbmodel.VoucherTypes.Select(m => new SelectListItem() { Text = m.VoucherTypeName, Value = m.TypeId.ToString() }).ToList();
            //if (completeMdl == null)
            return View(new AccountTrans() { vouchertypeList = vouchertype, accoutnTransaction = new List<TransModel>() });

            // return View(new AccountTrans() { vouchertypeList = vouchertype, accoutnTransaction = completeMdl.accoutnTransaction == null ? new List<TransModel>() : completeMdl.accoutnTransaction as List<TransModel>, AccountNO = completeMdl.AccountNO, Fromdate = completeMdl.Fromdate, ToDate = completeMdl.ToDate });
        }

        [HttpPost]
        public ActionResult AccountTrans(AccountTrans Tranfilter)
        {
            //int countit=_accountdbmodel.Transaction.Count();
            Tranfilter.vouchertypeList = _accountdbmodel.VoucherTypes.Select(m => new SelectListItem() { Text = m.VoucherTypeName, Value = m.TypeId.ToString() }).ToList();
            Tranfilter.accoutnTransaction = transdetails(Tranfilter);
            return View(Tranfilter);
        }
        public ActionResult Delete(string TRANSNO, string accno, string coorname, string fromDate, string toDate, int VoucherType)
        {

            Transaction Tran = _accountdbmodel.Transaction.FirstOrDefault(m => m.TransNO == TRANSNO);

            if (Tran != null)
            {
                if (Tran.RecupDone == true)
                {
                    if (_accountdbmodel.VoucherTypes.Where(m => m.TypeId == Tran.voucherType).Select(m => m.recoup).FirstOrDefault())
                    {
                        ImprestMaster impmstr = _accountdbmodel.ImprestMasters.Where(m => m.BankAccountNo == Tran.BankAccountNO).FirstOrDefault();
                        impmstr.Amount -= (decimal)Tran.Amount;

                        _accountdbmodel.Transaction.Remove(Tran);
                        _accountdbmodel.Database.ExecuteSqlCommand(@"update [AccTransaction] set RecupDone=0 where BankAccountNO = {0} and Recoupid={1}", Tran.BankAccountNO, Tran.Recoupid);
                        _accountdbmodel.SaveChanges();
                    }
                    else
                    {
                        List<Transaction> tempLIst = _accountdbmodel.Transaction.Where(m => m.BankAccountNO == Tran.BankAccountNO && m.brsDone == false).ToList();
                        Transaction retran=tempLIst.Where(m => m.Recoupid == Tran.Recoupid && (m.voucherType == 1 || m.voucherType == 5)).FirstOrDefault();
                        ImprestMaster impst= _accountdbmodel.ImprestMasters.FirstOrDefault(m=>m.BankAccountNo==Tran.BankAccountNO);
                        impst.Amount +=(decimal) Tran.Amount;
                        impst.Amount -= (decimal)Tran.Amount;
                        _accountdbmodel.Transaction.Remove(Tran);
                        _accountdbmodel.Transaction.Remove(retran);
                        _accountdbmodel.Database.ExecuteSqlCommand(@"update [AccTransaction] set RecupDone=0 where BankAccountNO = {0} and Recoupid={1}", Tran.BankAccountNO, Tran.Recoupid);
                        _accountdbmodel.SaveChanges();
                    }
                }
                else
                {
                    if (_accountdbmodel.VoucherTypes.Where(m => m.TypeId == Tran.voucherType).Select(m => m.deposit).FirstOrDefault())
                    {
                        ImprestMaster impmstr = _accountdbmodel.ImprestMasters.Where(m => m.BankAccountNo == Tran.BankAccountNO).FirstOrDefault();
                        impmstr.Amount -= (decimal)Tran.Amount;
                        _accountdbmodel.Transaction.Remove(Tran);
                        _accountdbmodel.SaveChanges();
                    }
                    else
                    {
                        ImprestMaster impmstr = _accountdbmodel.ImprestMasters.Where(m => m.BankAccountNo == Tran.BankAccountNO).FirstOrDefault();
                        impmstr.Amount += (decimal)Tran.Amount;
                        _accountdbmodel.Transaction.Remove(Tran);
                        _accountdbmodel.SaveChanges();
                    }
                    //_accountdbmodel.Transaction.Remove(Tran);
                    //_accountdbmodel.SaveChanges();
                }
            }
            AccountTrans Tranfilter = new AccountTrans();
            Tranfilter.AccountNO = accno.Trim();
            Tranfilter.Fromdate = fromDate.Trim();
            Tranfilter.ToDate = toDate.Trim();
            Tranfilter.coordnatorName = coorname.Trim();
            Tranfilter.VoucherTypeID = VoucherType;
            Tranfilter.vouchertypeList = _accountdbmodel.VoucherTypes.Select(m => new SelectListItem() { Text = m.VoucherTypeName, Value = m.TypeId.ToString() }).ToList();
            Tranfilter.accoutnTransaction = transdetails(Tranfilter);
            return View("AccountTrans", Tranfilter);
        }

        public ActionResult UnDelete(string TRANSNO, string accno, string coorname, string fromDate, string toDate, int VoucherType)
        {
            Transaction Tran = _accountdbmodel.Transaction.FirstOrDefault(m => m.TransNO == TRANSNO);
            Tran.deleted = false;
            _accountdbmodel.SaveChanges();

            AccountTrans Tranfilter = new AccountTrans();
            Tranfilter.AccountNO = accno.Trim();
            Tranfilter.Fromdate = fromDate.Trim();
            Tranfilter.ToDate = toDate.Trim();
            Tranfilter.coordnatorName = coorname.Trim();
            Tranfilter.VoucherTypeID = VoucherType;
            Tranfilter.showdeleted = true;
            Tranfilter.vouchertypeList = _accountdbmodel.VoucherTypes.Select(m => new SelectListItem() { Text = m.VoucherTypeName, Value = m.TypeId.ToString() }).ToList();
            Tranfilter.accoutnTransaction = transdetails(Tranfilter);

            return View("AccountTrans", Tranfilter);

        }
        public ActionResult Edit(string TRANSNO = null)
        {
            if (string.IsNullOrEmpty(TRANSNO))
                return RedirectToAction("AccountTrans");
            //var dd = Request.QueryString["VoucherType"];
            TempData["editRedirect"] = Request.RawUrl;
            Transaction trans = _accountdbmodel.Transaction.FirstOrDefault(m => m.TransNO == TRANSNO);


            trans.bnkdate = DBNull.Value.Equals(trans.bankDate) == true ? "" : Convert.ToDateTime(trans.bankDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

            return RedirectToAction("Index", new { TransNO = TRANSNO });
        }

        [HttpPost]
        public ActionResult Edit(Transaction tran)
        {
            var temptran = _accountdbmodel.Transaction.Where(m => m.TransNO == tran.TransNO).FirstOrDefault();
            if (temptran.RecupDone)
            {
                if (tran.Amount != temptran.Amount)
                {
                    Transaction temp = _accountdbmodel.Transaction.Where(m => m.BankAccountNO == tran.BankAccountNO && m.Recoupid == temptran.Recoupid && (m.voucherType == 1 || m.voucherType == 4)).FirstOrDefault();
                    _accountdbmodel.Transaction.Remove(temp);
                    _accountdbmodel.Database.ExecuteSqlCommand(@"update [AccTransaction] set RecupDone=0 where BankAccountNO = {0} and Recoupid={1}", temptran.BankAccountNO, temptran.Recoupid);
                    _accountdbmodel.SaveChanges();
                }
            }
            Transaction tempTrans = _accountdbmodel.Transaction.Where(m => m.TransNO == tran.TransNO.Trim()).FirstOrDefault();
            tempTrans.CommitmentNO = tran.CommitmentNO;
            tempTrans.voucherNo = tran.voucherNo;
            tempTrans.voucherType = tran.voucherType;
            tempTrans.ProjectNo = tran.ProjectNo;
            tempTrans.ProjectType = tran.ProjectType;
            tempTrans.narration = tran.narration;
            tempTrans.Remarks = tran.Remarks;
            tempTrans.ChequeNO = tran.ChequeNO;
            tempTrans.Amount = tran.Amount;
            tempTrans.AvailableBal = tran.AvailableBal;
            tempTrans.BankAccountNO = tran.BankAccountNO;
            tempTrans.supplier = tran.supplier == 0 ? 1 : tran.supplier;
            tempTrans.bankDate = DateTime.ParseExact(tran.bnkdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            _accountdbmodel.SaveChanges();
            ViewData["HeaderMsg"] = "Data Updated";
            return View("Close");
        }
        public ActionResult ExporttoExcel(AccountTrans Tranfilter)
        {
            Tranfilter.vouchertypeList = _accountdbmodel.VoucherTypes.Select(m => new SelectListItem() { Text = m.VoucherTypeName, Value = m.TypeId.ToString() }).ToList();
            Tranfilter.accoutnTransaction = transdetails(Tranfilter);
            if (Tranfilter.VoucherTypeID == 10)
            {
                return File(_IexporttoExcel.ExportClimbNS(new AccountTrans() { Fromdate = Tranfilter.Fromdate, ToDate = Tranfilter.ToDate, AccountNO = Tranfilter.AccountNO, coordnatorName = Tranfilter.coordnatorName, accoutnTransaction = Tranfilter.accoutnTransaction }), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", string.Format("{0} Transaction.xlsx", DateTime.Now));
            }
            return File(_IexporttoExcel.ExportTransaction(new AccountTrans() { Fromdate = Tranfilter.Fromdate, ToDate = Tranfilter.ToDate, AccountNO = Tranfilter.AccountNO, coordnatorName = Tranfilter.coordnatorName, accoutnTransaction = Tranfilter.accoutnTransaction }), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", string.Format("{0} Transaction.xlsx", DateTime.Now));

        }
        [HttpPost]
        public ActionResult Supplier(string searchStr)
        {
            var sList = _accountdbmodel.supplier.Where(m => m.Name.Contains(searchStr)).Select(m => new { Id = m.Id, Name = m.Name }).Take(5).ToList();
            return Json(sList, JsonRequestBehavior.AllowGet);
        }

        [NonAction]
        private bool createTran(string instid, string usrmail, out string TransactonNo, out string ermsgg)
        {
            string AccountNO = _accountdbmodel.ImprestMasters.Where(m => m.InstituteId == instid.Trim()).Select(m => m.BankAccountNo).FirstOrDefault();
            SqlParameter sInstid = new SqlParameter
            {
                ParameterName = "@instid",
                Value = instid
            };
            SqlParameter usrmailP = new SqlParameter
            {
                ParameterName = "@UserEmail",
                SqlDbType = SqlDbType.NVarChar,
                Size = 80,
                Value = usrmail
            };
            SqlParameter Accno = new SqlParameter
            {
                ParameterName = "@BankAccountNO",
                SqlDbType = SqlDbType.NVarChar,
                Size = 14,
                Value = AccountNO
            };

            SqlParameter genTrans = new SqlParameter
            {
                ParameterName = "@TransactionNo",
                SqlDbType = SqlDbType.NVarChar,
                Direction = ParameterDirection.Output,
                Size = 23
            };
            SqlParameter Isfailed = new SqlParameter
            {
                ParameterName = "@isFailed",
                SqlDbType = SqlDbType.Bit,
                Direction = ParameterDirection.Output
            };
            SqlParameter ermsg = new SqlParameter
            {
                ParameterName = "@ermsg",
                SqlDbType = SqlDbType.NVarChar,
                Direction = ParameterDirection.Output,
                Size = 10
            };
            //_accountdbmodel.Database.Connection.Close();
            try
            {
                _accountdbmodel.Database.ExecuteSqlCommand("exec createTran @instid,@UserEmail,@BankAccountNO,@TransactionNo OUT,@isFailed OUT,@ermsg OUT", sInstid, usrmailP, Accno, genTrans, Isfailed, ermsg);
                _accountdbmodel.Database.Connection.Close();
                TransactonNo = Convert.ToString(genTrans.Value);
                ermsgg = Convert.ToString(ermsg.Value);
                if ((bool)Isfailed.Value)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                // _Iexlogger.logError(_accountdbmodel, "createTran error -> " + ex.Message, User.Identity.Name);
                //return false;
                TransactonNo = "none";
                ermsgg = "AppCatch";
                return false;
            }

        }

        [NonAction]
        private bool commitTran(string TransNO, string ProjectNo, int? voucherType, string voucherNo, decimal? Amount, DateTime? bankDate, string ChequeNO, string narration, string Remarks, string commitno, string accno, int? projtype, string head, string description, out decimal? currentAmt, out decimal? availAmt, out string errmsg)
        {
            SqlParameter TransactionNoP = new SqlParameter
            {
                ParameterName = "@TransNo",
                Value = TransNO,
                SqlDbType = SqlDbType.NVarChar,
                Size = 23,
            };
            SqlParameter ProjectNOP = new SqlParameter
            {
                ParameterName = "@ProjectNo",
                SqlDbType = SqlDbType.NVarChar,
                Value = ProjectNo,
                Size = -1
            };
            SqlParameter voucherTypeP = new SqlParameter
            {
                ParameterName = "@voucherType",
                SqlDbType = SqlDbType.Int,
                Value = Convert.ToInt16(voucherType),
                Size = 1,
            };
            SqlParameter voucherNoP = new SqlParameter
            {
                ParameterName = "@voucherNo",
                SqlDbType = SqlDbType.NVarChar,
                Value = string.IsNullOrEmpty(voucherNo) == true ? (object)DBNull.Value : voucherNo,
                Size = -1
            };
            SqlParameter AmountP = new SqlParameter
            {
                ParameterName = "@Amount",
                SqlDbType = SqlDbType.Decimal,
                Precision = 18,
                Scale = 2,
                Value = Amount,
            };
            SqlParameter bankDateP = new SqlParameter
            {
                ParameterName = "@bankDate",
                SqlDbType = SqlDbType.DateTime,
                Value = bankDate == null ? (object)DBNull.Value : bankDate,
            };
            SqlParameter chequenoP = new SqlParameter
            {
                ParameterName = "@chequeno",
                SqlDbType = SqlDbType.NVarChar,
                Value = ChequeNO == null ? (object)DBNull.Value : ChequeNO,
                Size = -1
            };
            SqlParameter narrationP = new SqlParameter
            {
                ParameterName = "@narration",
                SqlDbType = SqlDbType.NVarChar,
                Size = 250,
                Value = narration == null ? (object)DBNull.Value : narration,
            };
            SqlParameter remarksP = new SqlParameter
            {
                ParameterName = "@remarks",
                SqlDbType = SqlDbType.NVarChar,
                Size = 250,
                Value = Remarks == null ? (object)DBNull.Value : Remarks
            };

            SqlParameter commitnoP = new SqlParameter
            {
                ParameterName = "@CommitmentNO",
                SqlDbType = SqlDbType.NVarChar,
                Size = -1,
                Value = commitno == null ? (object)DBNull.Value : commitno
            };

            SqlParameter AccountNO = new SqlParameter
            {
                ParameterName = "@BankAccountNO",
                SqlDbType = SqlDbType.NVarChar,
                Size = 14,
                Value = accno
            };

            SqlParameter projectType = new SqlParameter
            {
                ParameterName = "@ProjectType",
                SqlDbType = SqlDbType.Int,
                Value = projtype
            };
            SqlParameter Head = new SqlParameter
            {
                ParameterName = "@head",
                SqlDbType = SqlDbType.NVarChar,
                Size = 100,
                Value = head == null ? (object)DBNull.Value : head
            };
            SqlParameter Desc = new SqlParameter
            {
                ParameterName = "@desc",
                SqlDbType = SqlDbType.NVarChar,
                Size = 250,
                Value = description == null ? (object)DBNull.Value : description
            };
            SqlParameter currentamountP = new SqlParameter
            {
                ParameterName = "@currentamount",
                SqlDbType = SqlDbType.Decimal,
                Precision = 18,
                Scale = 2,
                Direction = ParameterDirection.Output,
            };
            SqlParameter avilableamountP = new SqlParameter
            {
                ParameterName = "@avilableamount",
                SqlDbType = SqlDbType.Decimal,
                Precision = 18,
                Scale = 2,
                Direction = ParameterDirection.Output,
            };
            SqlParameter errmsgP = new SqlParameter
            {
                ParameterName = "@errmsg",
                SqlDbType = SqlDbType.NVarChar,
                Size = 10,
                Direction = ParameterDirection.Output,
            };
            SqlParameter transdoneP = new SqlParameter
            {
                ParameterName = "@transdone",
                SqlDbType = SqlDbType.Bit,
                Direction = ParameterDirection.Output,
            };
            // try
            // {
            _accountdbmodel.Database.ExecuteSqlCommand("EXEC commitTran @TransNo, @ProjectNo, @voucherType, @voucherNo, @Amount,@bankDate, @chequeno, @narration, @remarks,@CommitmentNO,@BankAccountNO, @ProjectType,@head,@desc ,@currentamount out,@avilableamount out,@errmsg out,@transdone out",
        TransactionNoP, ProjectNOP, voucherTypeP, voucherNoP, AmountP, chequenoP, narrationP, remarksP, commitnoP, AccountNO, bankDateP, projectType, Desc, Head, currentamountP, avilableamountP, errmsgP, transdoneP);
            _accountdbmodel.Database.Connection.Close();
            currentAmt = Convert.ToDecimal(currentamountP.Value == DBNull.Value ? "0" : currentamountP.Value);
            availAmt = Convert.ToDecimal(avilableamountP.Value == DBNull.Value ? "0" : avilableamountP.Value);
            errmsg = Convert.ToString(errmsgP.Value);


            if ((bool)transdoneP.Value)
            {
                return true;
            }
            else
            {
                return false;
            }
            // }
            //catch 
            //{
            //    //_Iexlogger.logError(_accountdbmodel, "commit tran -> " + ex.Message, User.Identity.Name);
            //    //return false;
            //    currentAmt = 0;
            //    availAmt = 0;
            //    errmsg = "AppCatch";
            //    return false;
            //}
        }

        [NonAction]
        public List<Commitmdl> getProjectConsignmentDetails(DateTime? projectstartdate, string projectno, string CommitmentNO, string type, string boolfil, string filter)
        {
            try
            {
                int projectStYear = projectstartdate.Value.Year % 100;
                bool tableExist = true;
                List<Commitmdl> commitmentNumber = new List<Commitmdl>();

                //var paylist = new List<paymentDetails>();
                while (tableExist)
                {

                    DataTable paydetails = new DataTable();
                    string yearinstring = "0" + Convert.ToString(projectStYear);
                    yearinstring = yearinstring.Substring(yearinstring.Length - 2);
                    string tempy = "0" + Convert.ToString(projectStYear + 1);
                    tempy = tempy.Substring(tempy.Length - 2);
                    yearinstring = yearinstring + tempy;
                    if (_Facctservice.checkTableExistence(string.Format("VOU{0}", yearinstring)) == 1)
                    {
                        string cmd = string.Empty;
                        if (string.Equals(type, "CMSTLST"))
                        {
                            if (string.Equals(boolfil, "true"))
                            {
                                cmd = string.Format("SELECT [COMNO] FROM [VOU{0}] WHERE [ICCNO]='{2}' AND [COMNO] LIKE '%{1}%' AND [DISC] LIKE '%{3}%'", yearinstring, CommitmentNO, projectno, filter);

                            }
                            else
                            {
                                cmd = string.Format("SELECT [COMNO] FROM [VOU{0}] WHERE [ICCNO]='{2}' AND [COMNO] LIKE '%{1}%'", yearinstring, CommitmentNO, projectno);

                            }

                        }
                        if (string.Equals(type, "MSTLST"))
                        {
                            if (string.Equals(boolfil, "true"))
                            {
                                cmd = string.Format("SELECT [COMNO] FROM [VOU{0}] WHERE [NPRNO]='{2}' AND [COMNO] LIKE '%{1}%' AND [DISC] LIKE '%{3}%'", yearinstring, CommitmentNO, projectno, filter);


                            }
                            else
                            {
                                cmd = string.Format("SELECT [COMNO] FROM [VOU{0}] WHERE [NPRNO]='{2}' AND [COMNO] LIKE '%{1}%'", yearinstring, CommitmentNO, projectno);


                            }
                            //cmd = string.Format("SELECT [COMNO] FROM [VOU{0}] WHERE [NPRNO]='{2}' AND [COMNO] LIKE '%{1}%' AND [DISC] LIKE '%%'", yearinstring, CommitmentNO, projectno);

                        }
                        //string cmd = string.Format("SELECT [VRNO],[DATE], SUM([AMOUNT]) AS AMOUNT FROM VOU{0} WHERE [NPRNO] = '{1}' GROUP BY [VRNO],[DATE],[NPRNO] HAVING [VRNO] LIKE '%{2}%' ", yearinstring, projectno, voucherno);
                        //string cmd = string.Format("SELECT VRNO, AMOUNT, DATE FROM VOU{0} WHERE NPRNO = '{1}' AND VRNO LIKE '%{2}%'", yearinstring, projectno, consignment);
                        SqlDataReader reader = _Facctservice.FetchfromFacc(cmd);
                        paydetails.Load(reader);

                        List<Commitmdl> comitlist = (from DataRow dr in paydetails.Rows
                                                     select new Commitmdl()
                                                     {
                                                         CommitmentNO = Convert.ToString(dr["COMNO"].ToString()),
                                                     }).ToList();
                        commitmentNumber.AddRange(comitlist);


                        commitmentNumber = commitmentNumber.GroupBy(m => m.CommitmentNO).Select(m => new Commitmdl() { CommitmentNO = m.Key }).ToList();
                        if (commitmentNumber.Count > 7)
                            tableExist = false;
                    }
                    else
                    {
                        tableExist = false;
                    }
                    projectStYear++;
                }

                return commitmentNumber.Take(7).ToList();
            }
            catch
            {
                // _Iexlogger.logError(_accountdbmodel, "getProjectConsignmentDetails -> " + ex.Message, User.Identity.Name);

                return null;
            }


        }

        [NonAction]
        public List<VouDetail> getProjectVoucher(DateTime? projectstartdate, string voucher, string type, string filbool, string filterstr, string instid)
        {

            int projectStYear = projectstartdate.Value.Year % 100;
            bool tableExist = true;
            List<VouDetail> VoucherNumber = new List<VouDetail>();

            //var paylist = new List<paymentDetails>();
            while (tableExist)
            {
                string yearinstring = "0" + Convert.ToString(projectStYear);
                yearinstring = yearinstring.Substring(yearinstring.Length - 2);
                string tempy = "0" + Convert.ToString(projectStYear + 1);
                tempy = tempy.Substring(tempy.Length - 2);
                yearinstring = yearinstring + tempy;

                DataTable paydetails = new DataTable();
                // yearinstring = yearinstring + Convert.ToString(projectStYear + 1);
                if (_Facctservice.checkTableExistence(string.Format("VOU{0}", yearinstring)) == 1)
                {
                    string cmd = string.Empty;
                    if (string.Equals(type, "1"))//mstlst
                    {

                        if (string.Equals(filbool, "true"))
                        {
                            cmd = string.Format("SELECT a.[COMNO],a.[VRNO],a.[AMOUNT],a.[HEAD],a.[DATE],a.[CQNO],a.[PONO],a.[PART],a.[DISC],a.[NPRNO] FROM [FACCT].[dbo].[VOU{0}] a join [FACCT].[dbo].MSTLST c on a.NPRNO = c.NPRNO where c.INSTID ='{1}' and a.VRNO like '%{2}%' and a.DISC like '%{3}%'", yearinstring, instid, voucher, filterstr);

                        }
                        else
                        {
                            cmd = string.Format("SELECT a.[COMNO],a.[VRNO],a.[AMOUNT],a.[HEAD],a.[DATE],a.[CQNO],a.[PONO],a.[PART],a.[DISC],a.[NPRNO] FROM [FACCT].[dbo].[VOU{0}] a join [FACCT].[dbo].MSTLST c on a.NPRNO = c.NPRNO where c.INSTID ='{1}' and a.VRNO like '%{2}%'", yearinstring, instid, voucher);

                        }
                    }
                    if (string.Equals(type, "2"))
                    {

                        if (string.Equals(filbool, "true"))
                        {
                            cmd = string.Format("SELECT a.[COMNO],[VRNO],[AMOUNT],[HEAD],[DATE],[CQNO],[PONO],[PART],[DISC],a.ICCNO FROM [FACCT].[dbo].[VOU{0}] a join [FACCT].[dbo].CMSTLST b on a.ICCNO = b.CPRNO where INSTID ='{1}' AND a.VRNO like '%{2}%' AND a.DISC like '%{3}%'", yearinstring, instid, voucher, filterstr);

                        }
                        else
                        {
                            cmd = string.Format("SELECT a.[COMNO],[VRNO],[AMOUNT],[HEAD],[DATE],[CQNO],[PONO],[PART],[DISC],a.ICCNO FROM [FACCT].[dbo].[VOU{0}] a join [FACCT].[dbo].CMSTLST b on a.ICCNO = b.CPRNO where INSTID ='{1}' AND a.VRNO like '%{2}%'", yearinstring, instid, voucher);

                        }

                    }
                    //string cmd = string.Format("SELECT [VRNO],[DATE], SUM([AMOUNT]) AS AMOUNT FROM VOU{0} WHERE [NPRNO] = '{1}' GROUP BY [VRNO],[DATE],[NPRNO] HAVING [VRNO] LIKE '%{2}%' ", yearinstring, projectno, voucherno);
                    //string cmd = string.Format("SELECT VRNO, AMOUNT, DATE FROM VOU{0} WHERE NPRNO = '{1}' AND VRNO LIKE '%{2}%'", yearinstring, projectno, consignment);
                    SqlDataReader reader = _Facctservice.FetchfromFacc(cmd);
                    paydetails.Load(reader);
                    if (string.Equals(type, "1"))//mstlst
                    {
                        List<VouDetail> VouList = (from DataRow dr in paydetails.Rows
                                                   select new VouDetail()
                                                   {
                                                       vouno = Convert.ToString(dr["VRNO"]),
                                                       AMOUNT = Convert.ToDecimal(DBNull.Value.Equals(dr["AMOUNT"]) == true ? "0" : dr["AMOUNT"]),
                                                       PART = Convert.ToString(dr["PART"]),
                                                       DISC = Convert.ToString(dr["DISC"]),
                                                       HEAD = Convert.ToString(dr["HEAD"]),
                                                       PONO = Convert.ToString(dr["PONO"]),
                                                       CQNO = Convert.ToString(dr["CQNO"]),
                                                       Date = DBNull.Value.Equals(dr["DATE"]) == true ? "" : Convert.ToDateTime(dr["DATE"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                                                       projectNO = Convert.ToString(dr["NPRNO"]),
                                                       CommitNo = Convert.ToString(dr["COMNO"]),
                                                       ProjectType = 1
                                                   }).ToList();
                        VoucherNumber.AddRange(VouList);
                    }
                    if (string.Equals(type, "2"))//mstlst
                    {
                        List<VouDetail> VouList = (from DataRow dr in paydetails.Rows
                                                   select new VouDetail()
                                                   {
                                                       vouno = Convert.ToString(dr["VRNO"]),
                                                       AMOUNT = Convert.ToDecimal(DBNull.Value.Equals(dr["AMOUNT"]) == true ? "0" : dr["AMOUNT"]),
                                                       PART = Convert.ToString(dr["PART"]),
                                                       DISC = Convert.ToString(dr["DISC"]),
                                                       HEAD = Convert.ToString(dr["HEAD"]),
                                                       PONO = Convert.ToString(dr["PONO"]),
                                                       CQNO = Convert.ToString(dr["CQNO"]),
                                                       Date = DBNull.Value.Equals(dr["DATE"]) == true ? "" : Convert.ToDateTime(dr["DATE"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                                                       projectNO = Convert.ToString(dr["ICCNO"]),
                                                       CommitNo = Convert.ToString(dr["COMNO"]),
                                                       ProjectType = 2
                                                   }).ToList();
                        VoucherNumber.AddRange(VouList);
                    }
                    //VoucherNumber = VoucherNumber.GroupBy(m => m.vouno).Select(m => new VouDetail() { vouno = m.Key }).ToList();

                }
                else
                {
                    tableExist = false;
                }
                projectStYear++;
            }
            return VoucherNumber.Take(7).ToList();
        }

        public List<VouDetail> getVoucherOnly(DateTime? projectstartdate, string instid, string voucher, string filbool, string filterstr)
        {
            int projectStYear = projectstartdate.Value.Year % 100;
            bool tableExist = true;
            DataTable paydetails = new DataTable();
            List<VouDetail> VoucherNumber = new List<VouDetail>();
            while (tableExist)
            {
                string yearinstring = "0" + Convert.ToString(projectStYear);
                yearinstring = yearinstring.Substring(yearinstring.Length - 2);
                string tempy = "0" + Convert.ToString(projectStYear + 1);
                tempy = tempy.Substring(tempy.Length - 2);
                yearinstring = yearinstring + tempy;
                if (_Facctservice.checkTableExistence(string.Format("VOU{0}", yearinstring)) == 1)
                {
                    string cmd = string.Empty;
                    if (string.Equals(filbool, "true"))
                    {
                        cmd = string.Format("SELECT  a.VRNO,2 as [PT] FROM [FACCT].[dbo].[VOU{0}] a JOIN [FACCT].[dbo].CMSTLST b ON a.ICCNO = b.CPRNO WHERE INSTID LIKE '%{3}%' AND a.VRNO LIKE '%{1}%' AND a.DISC like '%{2}%' GROUP BY a.VRNO", yearinstring, voucher, filterstr, instid);

                    }
                    else
                    {
                        cmd = string.Format("SELECT  a.VRNO,2 as [PT] FROM [FACCT].[dbo].[VOU{0}] a JOIN [FACCT].[dbo].CMSTLST b ON a.ICCNO = b.CPRNO WHERE INSTID LIKE '%{2}%' AND a.VRNO LIKE '%{1}%' GROUP BY a.VRNO", yearinstring, voucher, instid);

                    }

                    SqlDataReader reader = _Facctservice.FetchfromFacc(cmd);
                    paydetails.Load(reader);
                    List<VouDetail> VouList = (from DataRow dr in paydetails.Rows
                                               select new VouDetail()
                                               {
                                                   vouno = Convert.ToString(dr["VRNO"]),
                                                   ProjectType = Convert.ToInt16(dr["PT"])
                                               }).ToList();
                    VoucherNumber.AddRange(VouList);
                    paydetails = new DataTable();
                    if (string.Equals(filbool, "true"))
                    {
                        cmd = string.Format("SELECT  a.VRNO,1 as [PT] FROM [FACCT].[dbo].[VOU{0}] a JOIN [FACCT].[dbo].MSTLST b ON a.NPRNO = b.NPRNO WHERE INSTID LIKE '%{3}%' AND a.VRNO LIKE '%{1}%' AND a.DISC like '%{2}%' GROUP BY a.VRNO", yearinstring, voucher, filterstr, instid);

                    }
                    else
                    {
                        cmd = string.Format("SELECT  a.VRNO,1 as [PT] FROM [FACCT].[dbo].[VOU{0}] a JOIN [FACCT].[dbo].MSTLST b ON a.NPRNO = b.NPRNO WHERE INSTID LIKE '%{2}%' AND a.VRNO LIKE '%{1}%' GROUP BY a.VRNO", yearinstring, voucher, instid);

                    }
                    SqlDataReader reader2 = _Facctservice.FetchfromFacc(cmd);
                    paydetails.Load(reader2);
                    List<VouDetail> VouList2 = (from DataRow dr in paydetails.Rows
                                                select new VouDetail()
                                                {
                                                    vouno = Convert.ToString(dr["VRNO"]),
                                                    ProjectType = Convert.ToInt16(dr["PT"])
                                                }).ToList();
                    VoucherNumber.AddRange(VouList2);
                    if (VoucherNumber.Count > 5)
                    {
                        tableExist = false;
                    }
                }
                else
                {
                    tableExist = false;
                }
                projectStYear++;
            }
            return VoucherNumber.Take(5).ToList();
        }
        //[NonAction]
        //public List<VouDetail> getVoucherDetails(DateTime? projectstartdate, string projectno, string CommitmentNO, string voucher, string type)
        //{
        //    int projectStYear = projectstartdate.Value.Year % 100;
        //    bool tableExist = true;
        //    List<VouDetail> VoucherDetails = new List<VouDetail>();

        //    //var paylist = new List<paymentDetails>();
        //    while (tableExist)
        //    {
        //        string yearinstring = "0" + Convert.ToString(projectStYear);
        //        yearinstring = yearinstring.Substring(yearinstring.Length - 2);
        //        string tempy = "0" + Convert.ToString(projectStYear + 1);
        //        tempy = tempy.Substring(tempy.Length - 2);
        //        yearinstring = yearinstring + tempy;

        //        DataTable paydetails = new DataTable();
        //        // yearinstring = yearinstring + Convert.ToString(projectStYear + 1);
        //        if (_Facctservice.checkTableExistence(string.Format("VOU{0}", yearinstring)) == 1)
        //        {
        //            string cmd = string.Empty;
        //            if (string.Equals(type, "CMSTLST"))
        //            {
        //                if (string.IsNullOrEmpty(CommitmentNO))
        //                {
        //                    cmd = string.Format("SELECT [VRNO],[AMOUNT],[HEAD],[DATE],[CQNO],[PONO],[PART],[DISC] FROM [VOU{0}] WHERE  [ICCNO] = '{2}' AND [VRNO] LIKE '%{3}%' ", yearinstring, CommitmentNO, projectno, voucher);

        //                }
        //                else
        //                {
        //                    cmd = string.Format("SELECT [VRNO],[AMOUNT],[HEAD],[DATE],[CQNO],[PONO],[PART],[DISC] FROM [VOU{0}] WHERE [COMNO] = '{1}' AND [ICCNO] = '{2}' AND [VRNO] LIKE '%{3}%' ", yearinstring, CommitmentNO, projectno, voucher);

        //                }
        //            }
        //            if (string.Equals(type, "MSTLST"))
        //            {
        //                if (string.IsNullOrEmpty(CommitmentNO))
        //                {
        //                    cmd = string.Format("SELECT [VRNO],[AMOUNT],[HEAD],[DATE],[CQNO],[PONO],[PART],[DISC] FROM [VOU{0}] WHERE  [NPRNO] = '{2}' AND [VRNO] LIKE '%{3}%' ", yearinstring, CommitmentNO, projectno, voucher.Replace("b", "").Replace("B", ""));

        //                }
        //                else
        //                {
        //                    cmd = string.Format("SELECT [VRNO],[AMOUNT],[HEAD],[DATE],[CQNO],[PONO],[PART],[DISC] FROM [VOU{0}] WHERE [COMNO] = '{1}' AND [NPRNO] = '{2}' AND [VRNO] LIKE '%{3}%' ", yearinstring, CommitmentNO, projectno, voucher.Replace("b", "").Replace("B", ""));

        //                }
        //            }
        //            //string cmd = string.Format("SELECT [VRNO],[AMOUNT],[PART],[DISC] FROM [VOU{0}] WHERE [COMNO] = '{1}' AND [NPRNO] = '{2}' AND [VRNO] LIKE '%{3}%'", yearinstring, CommitmentNO, projectno, voucher);
        //            //string cmd = string.Format("SELECT [VRNO],[DATE], SUM([AMOUNT]) AS AMOUNT FROM VOU{0} WHERE [NPRNO] = '{1}' GROUP BY [VRNO],[DATE],[NPRNO] HAVING [VRNO] LIKE '%{2}%' ", yearinstring, projectno, voucherno);
        //            //string cmd = string.Format("SELECT VRNO, AMOUNT, DATE FROM VOU{0} WHERE NPRNO = '{1}' AND VRNO LIKE '%{2}%'", yearinstring, projectno, consignment);
        //            SqlDataReader reader = _Facctservice.FetchfromFacc(cmd);
        //            paydetails.Load(reader);
        //            List<VouDetail> Voudetails = (from DataRow dr in paydetails.Rows
        //                                          select new VouDetail()
        //                                          {
        //                                              vouno = Convert.ToString(dr["VRNO"]),
        //                                              AMOUNT = Convert.ToDecimal(DBNull.Value.Equals(dr["AMOUNT"]) == true ? "0" : dr["AMOUNT"]),
        //                                              PART = Convert.ToString(dr["PART"]),
        //                                              DISC = Convert.ToString(dr["DISC"]),
        //                                              HEAD = Convert.ToString(dr["HEAD"]),
        //                                              PONO = Convert.ToString(dr["PONO"]),
        //                                              CQNO = Convert.ToString(dr["CQNO"]),
        //                                              Date = DBNull.Value.Equals(dr["DATE"]) == true ? "" : Convert.ToDateTime(dr["DATE"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
        //                                              //Bankdate = DateTime.ParseExact(transaction.bnkdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        //                                          }).ToList();
        //            VoucherDetails.AddRange(Voudetails);
        //        }
        //        else
        //        {
        //            tableExist = false;
        //        }
        //        projectStYear++;
        //    }
        //    return VoucherDetails;

        //}
        [NonAction]
        public IEnumerable<TransModel> transdetails(AccountTrans Tranfilter)
        {
            DateTime fromdate = DateTime.Now;
            if (!string.IsNullOrEmpty(Tranfilter.Fromdate))
            {
                fromdate = DateTime.ParseExact(Tranfilter.Fromdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }

            DateTime todate = DateTime.ParseExact(Tranfilter.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            
            IEnumerable<TransModel> accounttrans = new List<TransModel>();
            var tempList = _accountdbmodel.Transaction.Where(m => m.BankAccountNO == Tranfilter.AccountNO && (m.bankDate >= fromdate && m.bankDate <= todate)).ToList();
            if(Tranfilter.VoucherTypeID!=10)
            tempList=tempList.Where(m => m.voucherType == Tranfilter.VoucherTypeID).ToList();
            
            ////special filteration goes here
            
            if (Tranfilter.cns)
            {
                if (Tranfilter.Allacc)
                {
                    tempList = _accountdbmodel.Transaction.Where(m => (m.bankDate >= fromdate && m.bankDate <= todate) && m.CNS == true).ToList();
                }
                else
                {
                    tempList = tempList.Where(m => m.CNS == true).ToList();
                }
            }
          

            if (Tranfilter.ShowBRS)
            {
                tempList = tempList.Where(m => m.brsDone == true).ToList();
            }
            accounttrans = (from tran in tempList
                            join vt in _accountdbmodel.VoucherTypes on tran.voucherType equals vt.TypeId
                            join sup in _accountdbmodel.supplier on tran.supplier equals sup.Id
                            where tran.TransCommited == true
                            orderby tran.brsDone descending, tran.bankDate, tran.orderId
                            select new TransModel()
                            {
                                BankAccountNO = tran.BankAccountNO,
                                ProjectNo = tran.ProjectNo,
                                bankDate = tran.bankDate,
                                Amount = tran.Amount,
                                ChequeNO = tran.ChequeNO,
                                CommitmentNO = tran.CommitmentNO,
                                voucherNo = tran.voucherNo,
                                voucherType = tran.voucherType,
                                voucherTypeStr = vt.VoucherTypeName,
                                narration = tran.narration,
                                Remarks = tran.Remarks,
                                head = tran.Head,
                                desc = tran.Description,
                                TransNO = tran.TransNO,
                                deleted = tran.deleted,
                                currentBal = tran.currentBal,
                                AvailableBal = tran.AvailableBal,
                                brsDone = tran.brsDone,
                                orderId = tran.orderId,
                                bankPart = tran.BankPart,
                                CoordinatorName = Tranfilter.coordnatorName,
                                Supplier = sup.Name,
                                Recoupid = tran.Recoupid,
                                credit = vt.deposit,
                                CNS = vt.cns,
                                recoup = vt.recoup,
                            }).ToList();
            //if (Tranfilter.VoucherTypeID == 10 && Tranfilter.cns != true)// alltransaction
            //{

            //    if (Tranfilter.ShowBRS)
            //    {
            //accounttrans = (from tran in tempList
            //                join vt in _accountdbmodel.VoucherTypes on tran.voucherType equals vt.TypeId
            //                join sup in _accountdbmodel.supplier on tran.supplier equals sup.Id
            //                where tran.TransCommited == true && tran.brsDone == true
            //                orderby tran.brsDone descending, tran.bankDate, tran.orderId
            //                select new TransModel()
            //                {
            //                    BankAccountNO = tran.BankAccountNO,
            //                    ProjectNo = tran.ProjectNo,
            //                    bankDate = tran.bankDate,
            //                    Amount = tran.Amount,
            //                    ChequeNO = tran.ChequeNO,
            //                    CommitmentNO = tran.CommitmentNO,
            //                    voucherNo = tran.voucherNo,
            //                    voucherType = tran.voucherType,
            //                    voucherTypeStr = vt.VoucherTypeName,
            //                    narration = tran.narration,
            //                    Remarks = tran.Remarks,
            //                    head = tran.Head,
            //                    desc = tran.Description,
            //                    TransNO = tran.TransNO,
            //                    deleted = tran.deleted,
            //                    currentBal = tran.currentBal,
            //                    AvailableBal = tran.AvailableBal,
            //                    brsDone = tran.brsDone,
            //                    orderId = tran.orderId,
            //                    bankPart = tran.BankPart,
            //                    CoordinatorName = Tranfilter.coordnatorName,
            //                    Supplier = sup.Name,
            //                    Recoupid = tran.Recoupid,
            //                    credit = vt.deposit,
            //                    CNS = vt.cns,
            //                    recoup = vt.recoup,
            //                }).ToList();
            //    }
            //    else
            //    {
            //        accounttrans = (from tran in tempList
            //                        join vt in _accountdbmodel.VoucherTypes on tran.voucherType equals vt.TypeId
            //                        join sup in _accountdbmodel.supplier on tran.supplier equals sup.Id
            //                        where tran.TransCommited == true
            //                        orderby tran.brsDone descending, tran.bankDate, tran.orderId
            //                        select new TransModel()
            //                        {
            //                            BankAccountNO = tran.BankAccountNO,
            //                            ProjectNo = tran.ProjectNo,
            //                            bankDate = tran.bankDate,
            //                            Amount = tran.Amount,
            //                            ChequeNO = tran.ChequeNO,
            //                            CommitmentNO = tran.CommitmentNO,
            //                            voucherNo = tran.voucherNo,
            //                            voucherType = tran.voucherType,
            //                            voucherTypeStr = vt.VoucherTypeName,
            //                            narration = tran.narration,
            //                            Remarks = tran.Remarks,
            //                            head = tran.Head,
            //                            desc = tran.Description,
            //                            TransNO = tran.TransNO,
            //                            deleted = tran.deleted,
            //                            currentBal = tran.currentBal,
            //                            AvailableBal = tran.AvailableBal,
            //                            brsDone = tran.brsDone,
            //                            orderId = tran.orderId,
            //                            bankPart = tran.BankPart,
            //                            CoordinatorName = Tranfilter.coordnatorName,
            //                            Supplier = sup.Name,
            //                            Recoupid = tran.Recoupid,
            //                            credit = vt.deposit,
            //                            CNS = vt.cns,
            //                            recoup = vt.recoup,
            //                        }).ToList();
            //    }
            //}

            //else
            //{
            //    if (Tranfilter.cns && Tranfilter.Allacc)
            //    {
            //        accounttrans = (from tran in _accountdbmodel.Transaction
            //                        join vt in _accountdbmodel.VoucherTypes on tran.voucherType equals vt.TypeId
            //                        join sup in _accountdbmodel.supplier on tran.supplier equals sup.Id
            //                        join coor in _accountdbmodel.ImprestMasters on tran.INSTID equals coor.InstituteId
            //                        where tran.CNS == true && tran.TransCommited == true && tran.deleted == false
            //                        orderby tran.brsDone descending, tran.bankDate, tran.orderId
            //                        select new TransModel()
            //                        {
            //                            BankAccountNO = tran.BankAccountNO,
            //                            ProjectNo = tran.ProjectNo,
            //                            bankDate = tran.bankDate,
            //                            Amount = tran.Amount,
            //                            ChequeNO = tran.ChequeNO,
            //                            CommitmentNO = tran.CommitmentNO,
            //                            voucherNo = tran.voucherNo,
            //                            voucherType = tran.voucherType,
            //                            voucherTypeStr = vt.VoucherTypeName,
            //                            narration = tran.narration,
            //                            Remarks = tran.Remarks,
            //                            head = tran.Head,
            //                            desc = tran.Description,
            //                            TransNO = tran.TransNO,
            //                            deleted = tran.deleted,
            //                            currentBal = tran.currentBal,
            //                            AvailableBal = tran.AvailableBal,
            //                            brsDone = tran.brsDone,
            //                            orderId = tran.orderId,
            //                            bankPart = tran.BankPart,
            //                            CoordinatorName = coor.CoordinatorName,
            //                            Supplier = sup.Name,
            //                            Recoupid = tran.Recoupid,
            //                            credit = vt.deposit,
            //                            CNS = vt.cns,
            //                            recoup = vt.recoup,
            //                        }).ToList();


            //        return accounttrans;
            //    }
            //    else if (Tranfilter.cns && !Tranfilter.Allacc)///cns for one account
            //    {
            //        accounttrans = (from tran in tempList
            //                        join vt in _accountdbmodel.VoucherTypes on tran.voucherType equals vt.TypeId
            //                        join sup in _accountdbmodel.supplier on tran.supplier equals sup.Id
            //                        join coor in _accountdbmodel.ImprestMasters on tran.INSTID equals coor.InstituteId
            //                        where tran.CNS == true && tran.TransCommited == true && tran.deleted == false
            //                        orderby tran.brsDone descending, tran.bankDate, tran.orderId
            //                        select new TransModel()
            //                        {
            //                            BankAccountNO = tran.BankAccountNO,
            //                            ProjectNo = tran.ProjectNo,
            //                            bankDate = tran.bankDate,
            //                            Amount = tran.Amount,
            //                            ChequeNO = tran.ChequeNO,
            //                            CommitmentNO = tran.CommitmentNO,
            //                            voucherNo = tran.voucherNo,
            //                            voucherType = tran.voucherType,
            //                            voucherTypeStr = vt.VoucherTypeName,
            //                            narration = tran.narration,
            //                            Remarks = tran.Remarks,
            //                            head = tran.Head,
            //                            desc = tran.Description,
            //                            TransNO = tran.TransNO,
            //                            deleted = tran.deleted,
            //                            currentBal = tran.currentBal,
            //                            AvailableBal = tran.AvailableBal,
            //                            brsDone = tran.brsDone,
            //                            orderId = tran.orderId,
            //                            bankPart = tran.BankPart,
            //                            CoordinatorName = coor.CoordinatorName,
            //                            Supplier = sup.Name,
            //                            Recoupid = tran.Recoupid,
            //                            credit = vt.deposit,
            //                            CNS = vt.cns,
            //                            recoup = vt.recoup,
            //                        }).ToList();
            //        //Taccounttrans.AddRange(accounttrans);
            //        return accounttrans;
            //    }

            //    else if (Tranfilter.ShowBRS)
            //    {
            //        accounttrans = (from tran in tempList
            //                        join vt in _accountdbmodel.VoucherTypes on tran.voucherType equals vt.TypeId
            //                        join sup in _accountdbmodel.supplier on tran.supplier equals sup.Id

            //                        where tran.voucherType == Tranfilter.VoucherTypeID && tran.TransCommited == true && tran.brsDone == true
            //                        orderby tran.brsDone descending, tran.bankDate, tran.orderId
            //                        select new TransModel()
            //                        {
            //                            BankAccountNO = tran.BankAccountNO,
            //                            ProjectNo = tran.ProjectNo,
            //                            bankDate = tran.bankDate,
            //                            Amount = tran.Amount,
            //                            ChequeNO = tran.ChequeNO,
            //                            CommitmentNO = tran.CommitmentNO,
            //                            voucherNo = tran.voucherNo,
            //                            voucherType = tran.voucherType,
            //                            voucherTypeStr = vt.VoucherTypeName,
            //                            narration = tran.narration,
            //                            Remarks = tran.Remarks,
            //                            head = tran.Head,
            //                            desc = tran.Description,
            //                            TransNO = tran.TransNO,
            //                            deleted = tran.deleted,
            //                            currentBal = tran.currentBal,
            //                            AvailableBal = tran.AvailableBal,
            //                            brsDone = tran.brsDone,
            //                            orderId = tran.orderId,
            //                            bankPart = tran.BankPart,
            //                            CoordinatorName = Tranfilter.coordnatorName,
            //                            Supplier = sup.Name,
            //                            Recoupid = tran.Recoupid,
            //                            credit = vt.deposit,
            //                            CNS = vt.cns,
            //                            recoup = vt.recoup,
            //                        }).ToList();
            //    }
            //    else
            //    {
            //        accounttrans = (from tran in tempList
            //                        join vt in _accountdbmodel.VoucherTypes on tran.voucherType equals vt.TypeId
            //                        join sup in _accountdbmodel.supplier on tran.supplier equals sup.Id
            //                        where tran.voucherType == Tranfilter.VoucherTypeID && tran.TransCommited == true && tran.deleted == false
            //                        orderby tran.brsDone descending, tran.bankDate, tran.orderId
            //                        select new TransModel()
            //                        {
            //                            BankAccountNO = tran.BankAccountNO,
            //                            ProjectNo = tran.ProjectNo,
            //                            bankDate = tran.bankDate,
            //                            Amount = tran.Amount,
            //                            ChequeNO = tran.ChequeNO,
            //                            CommitmentNO = tran.CommitmentNO,
            //                            voucherNo = tran.voucherNo,
            //                            voucherType = tran.voucherType,
            //                            voucherTypeStr = vt.VoucherTypeName,
            //                            narration = tran.narration,
            //                            Remarks = tran.Remarks,
            //                            head = tran.Head,
            //                            desc = tran.Description,
            //                            TransNO = tran.TransNO,
            //                            deleted = tran.deleted,
            //                            currentBal = tran.currentBal,
            //                            AvailableBal = tran.AvailableBal,
            //                            brsDone = tran.brsDone,
            //                            orderId = tran.orderId,
            //                            bankPart = tran.BankPart,
            //                            CoordinatorName = Tranfilter.coordnatorName,
            //                            Supplier = sup.Name,
            //                            Recoupid = tran.Recoupid,
            //                            credit = vt.deposit,
            //                            CNS = vt.cns,
            //                            recoup = vt.recoup,
            //                        }).ToList();
            //    }
            //}
            //Taccounttrans.AddRange(accounttrans);
            return accounttrans;
        }
        public ActionResult checkTransaction()
        {
            List<Transaction> orderedList = new List<Transaction>();
            List<Transaction> updatedorderedList = new List<Transaction>();
            List<Transaction> transList = _accountdbmodel.Transaction.Where(m => m.BankAccountNO == "2722101012097" && (m.bankDate >= new DateTime(2017, 2, 1) && m.bankDate <= new DateTime(2017, 2, 28)) && m.deleted == false).OrderBy(m => m.bankDate).ToList();
            transList.OrderBy(m => m.bankDate);
            while (transList.Count > 0)
            {
                Transaction firstRecord = transList.FirstOrDefault();
                List<Transaction> temp = transList.Where(m => m.bankDate == firstRecord.bankDate).OrderBy(m => m.orderId).ToList();
                orderedList.AddRange(temp);
                transList.RemoveRange(0, temp.Count);
            }
            decimal amount = (decimal)_accountdbmodel.ImprestMasters.Where(m => m.BankAccountNo == "2722101012097").Select(m => m.Amount).FirstOrDefault();

            foreach (Transaction add in orderedList)
            {
                if (add.voucherType == 1 || add.voucherType == 8)
                {
                    add.AvailableBal = amount;
                    add.currentBal = amount + add.Amount;
                    amount = (decimal)add.currentBal;
                    add.TransUpdated = DateTime.Now;
                }
                else
                {
                    add.AvailableBal = amount;
                    add.currentBal = amount - add.Amount;
                    amount = (decimal)add.currentBal;
                    add.TransUpdated = DateTime.Now;
                }
                updatedorderedList.Add(add);
            }

            List<TransModel> converted = updatedorderedList.Select(m => new TransModel()
            {
                Amount = m.Amount,
                AvailableBal = m.AvailableBal,
                BankAccountNO = m.BankAccountNO,
                WITHDRAWALS = m.WITHDRAWALS,
                bankDate = m.bankDate,
                ChequeNO = m.ChequeNO,
                CommitmentNO = m.CommitmentNO,
                currentBal = m.currentBal,
                ProjectNo = m.ProjectNo,
                ProjectType = m.ProjectType,
                voucherType = m.voucherType
            }).ToList();
            return View("AccountTrans", new AccountTrans() { accoutnTransaction = converted });

        }
    }
}