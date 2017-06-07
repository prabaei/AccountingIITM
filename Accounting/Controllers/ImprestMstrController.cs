using Accounting.AccountAuthorize;
using Accounting.data.Database;
using Accounting.data.Database.FACCT;
using Accounting.data.Database.FoxOffice;
using Accounting.data.Database.PCFACCT;
using Accounting.data.services.Export;
using Accounting.data.services.Import;
using Accounting.Models.ImprestMstr;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Accounting.Controllers
{
    [AccountAutherize]
    public class ImprestMstrController : Controller
    {
        
        private readonly IFacctService _ifacctservice;
        private readonly IPCFACCTService _ipcfactservice;
        private readonly IAccountingDbModel _iaccountingDbModelservice;
        private readonly IExportToExcel _IexporttoExcel;
        public readonly IImportExcel _IImportExcel;
        public ImprestMstrController( IFacctService ifactservice, IPCFACCTService ipcfactservice, IAccountingDbModel AccountDb, IExportToExcel Export,IImportExcel importexcel)
        {
            
            _ifacctservice = ifactservice;
            _ipcfactservice = ipcfactservice;
            _iaccountingDbModelservice = AccountDb;
            _IexporttoExcel = Export;
            _IImportExcel = importexcel;
            _ipcfactservice.openconnection();
            _ifacctservice.openconnection();
            
        }
        // GET: ImprestMstr
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult INSTID(string searchStr)
        {
            // string cmd = string.Format("SELECT TOP 5 [IIRNO] FROM CO_NME WHERE [IIRNO] LIKE '%{0}%'",searchStr);
            //string cmd = string.Format("SELECT TOP 7 [INSTID],[COOR_NAME]  FROM [COORCOD] WHERE INSTID LIKE '%{0}%'", searchStr);
            string cmd = string.Format("SELECT TOP 7 [NAME],[IIRNO] FROM [CO_NME] WHERE [IIRNO] LIKE '%{0}%'", searchStr);
            SqlDataReader st = _ipcfactservice.getIPCFFACTdata(cmd);
            DataTable dt = new DataTable();
            dt.Load(st);
            var IIROList = (from DataRow dr in dt.Rows
                            select new
                            {
                                INSTID = Convert.ToString(dr["IIRNO"].ToString()),
                                Name = Convert.ToString(dr["NAME"]),
                                
                            }).ToList();
            return Json(IIROList, JsonRequestBehavior.AllowGet);
        }
       
        public ActionResult Edit(string Accountno)
        {
            ImprestMaster temp = _iaccountingDbModelservice.ImprestMasters.FirstOrDefault(m => m.BankAccountNo == Accountno);
            //DataTable dt = new DataTable();
            //string cmd = string.Format("SELECT [NAME],[IIRNO],[DESIG],[DEPT] FROM [CO_NME] WHERE [IIRNO] LIKE '%{0}%'", temp.InstituteId.Trim());

            //SqlDataReader st = _ipcfactservice.getIPCFFACTdata(cmd);
            //dt.Clear();
            //dt.Load(st);
            //var coorddetail = (from DataRow datro in dt.Rows
            //                   select new
            //                   {
            //                       INSTID = Convert.ToString(datro["IIRNO"]).TrimStart('0'),
            //                       Name = datro["NAME"].ToString(),
            //                       Department = datro["DEPT"].ToString(),
            //                       desig = datro["DESIG"].ToString(),
            //                   }).FirstOrDefault();
            //temp.Department = coorddetail.Department;
            //temp.Designation = coorddetail.desig;
            return View("Index", temp);
        }

        [HttpPost]
        public JsonResult INSTIDSelect(string searchStr)
        {
            //string cmd = string.Format("SELECT count(INSTID) AS TCOUNT FROM [COORCOD] WHERE INSTID LIKE '%{0}%'", searchStr);
            string cmd = string.Format("SELECT count(IIRNO) AS TCOUNT FROM [CO_NME] WHERE [IIRNO] LIKE '%{0}%'", searchStr);
            SqlDataReader st = _ipcfactservice.getIPCFFACTdata(cmd);
            DataTable dt = new DataTable();
            dt.Load(st);
            DataRow dr = dt.Rows[0];
            int count = Convert.ToInt16(dr["TCOUNT"]);
            if (count == 1)
            {
                cmd = string.Format("SELECT [NAME],[IIRNO],[DESIG],[DEPT] FROM [CO_NME] WHERE [IIRNO] LIKE '%{0}%'", searchStr);

                st = _ipcfactservice.getIPCFFACTdata(cmd);
                dt.Clear();
                dt.Load(st);
                var coorddetail = (from DataRow datro in dt.Rows
                                   select new
                                   {
                                       INSTID = Convert.ToString(datro["IIRNO"]).TrimStart('0'),
                                       Name = datro["NAME"].ToString(),
                                       Department = datro["DEPT"].ToString(),
                                       desig = datro["DESIG"].ToString(),
                                   }).FirstOrDefault();
              ImprestMaster impmstr =  _iaccountingDbModelservice.ImprestMasters.FirstOrDefault(m => m.InstituteId == searchStr.Trim());
                return Json(new { message = "success", cordDetails = coorddetail,accountdetail = impmstr }, JsonRequestBehavior.AllowGet);
                
            }
            else
            {
                // check INSTID no ..
                return Json(new { message = "error" }, JsonRequestBehavior.AllowGet);
            }
            // return null;
        }

        public ActionResult AccountGrid()
        {
            IEnumerable<ImprestMaster> impmstrList = _iaccountingDbModelservice.ImprestMasters.ToList() ;
            return View(impmstrList);
        }

        public ActionResult ExporttoExcel()
        {
            return File(_IexporttoExcel.ExportAccountDetails(_iaccountingDbModelservice.ImprestMasters.ToList()), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", string.Format("{0} AccountDetail.xlsx", DateTime.Now));
        }

        [HttpPost]
        [AccountAutherize(Roles ="admin")]
        public ActionResult ImportFromExcel()
        {
            var file=Request.Files["ImportAccountDetails"];
            if (file != null)
            {
                if (string.Equals(file.ContentType, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")){
                    _IImportExcel.ImportAccountDetails(file.InputStream);
                }
                else
                {

                }
            }
            //if (file != null && file.ContentLength > 0)
            //{
            //}
                return RedirectToAction("AccountGrid");
        }

        [HttpPost]
        public JsonResult AccountDetails(ImprestMaster Imprest)
        {
            ImprestMaster search = _iaccountingDbModelservice.ImprestMasters.Where(m => m.BankAccountNo== Imprest.BankAccountNo).FirstOrDefault();
            if (search == null)
            {
                _iaccountingDbModelservice.ImprestMasters.Add(Imprest);
               if(_iaccountingDbModelservice.SaveChanges() == 1)
                {
                    return Json(new { save = "s",message="AccountCreated"}, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { save = "f", message = "sqlError" }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                //ImprestMaster temp = _iaccountingDbModelservice.ImprestMasters.FirstOrDefault(m => m.InstituteId == search.InstituteId);
                search.CUSTID = Imprest.CUSTID;
                search.BankAccountNo = Imprest.BankAccountNo;
                search.CardNO = Imprest.CardNO;
                search.Amount = Imprest.Amount;
                search.Limit = Imprest.Limit;
                search.InstituteId = Imprest.InstituteId;
                search.Email = Imprest.Email;
                _iaccountingDbModelservice.SaveChanges();
                return Json(new { save = "s", message = "Account Updated" }, JsonRequestBehavior.AllowGet);

            }
        } 
    }
}