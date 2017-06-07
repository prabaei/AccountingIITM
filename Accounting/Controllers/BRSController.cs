using Accounting.AccountAuthorize;
using Accounting.data.Database;
using Accounting.data.services.Import;
using Accounting.Models.BRS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TestProcess;

namespace Accounting.Controllers
{
    [AccountAutherize]
    public class BRSController : Controller
    {
        readonly IImportTxt _importtxt;
        readonly IAccountingDbModel _accoutdbmodel;
        public BRSController(IImportTxt importtxt, IAccountingDbModel AccountDbmodel)
        {
            _importtxt = importtxt;
            _accoutdbmodel = AccountDbmodel;
        }
        // GET: BRS
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult importBankTxt()
        {
            return null;
        }

        public JsonResult removeTrans(string transno = null)
        {
            Transaction removeT = _accoutdbmodel.Transaction.FirstOrDefault(m => m.TransNO == transno.Trim());
            ImprestMaster impst = _accoutdbmodel.ImprestMasters.Where(m => m.BankAccountNo == removeT.BankAccountNO).FirstOrDefault();
            if (_accoutdbmodel.VoucherTypes.Where(m => m.TypeId == removeT.voucherType).Select(m => m.deposit).FirstOrDefault())
            {
                impst.Amount -= (decimal)removeT.Amount;
            }
            else
            {
                impst.Amount += (decimal)removeT.Amount;
            }
            _accoutdbmodel.Transaction.Remove(removeT);
            int result = _accoutdbmodel.SaveChanges();
            if (result == 2)
            {
                return Json(new { job = "done" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { job = "notdone" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult staleChq(string transno)
        {
            Transaction tran = _accoutdbmodel.Transaction.Where(m => m.TransNO == transno.Trim()).FirstOrDefault();
            ChequeNotDep cnd = _accoutdbmodel.cheqnotsub.Where(m => m.Tranno == transno.Trim()).FirstOrDefault();
            tran.brsDone = true;
            _accoutdbmodel.cheqnotsub.Remove(cnd);
            int result = _accoutdbmodel.SaveChanges();
            if (result == 2)
                return Json(new { job = "done" }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { job = "notdone" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChangeType(string transno, string voucherType)
        {
            int result2 = 0;
            Transaction removeT = _accoutdbmodel.Transaction.FirstOrDefault(m => m.TransNO == transno.Trim());
            removeT.voucherType = Convert.ToInt16(voucherType);
            int result = _accoutdbmodel.SaveChanges();
            int vt = Convert.ToInt16(voucherType);
            var vouch = _accoutdbmodel.VoucherTypes.Where(m => m.TypeId == vt).FirstOrDefault();
            if (vouch.cq)
            {
                var temptran = _accoutdbmodel.Transaction.Where(m => m.TransNO == transno).FirstOrDefault();
                ChequeNotDep cnd = new ChequeNotDep();
                cnd.Tranno = temptran.TransNO;
                cnd.Accno = temptran.BankAccountNO;

                _accoutdbmodel.cheqnotsub.Add(cnd);
                result2 = _accoutdbmodel.SaveChanges();
            }
            if (result == 1 && result2 == 1)
            {
                return Json(new { job = "done" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { job = "notdone" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ClimbNotSubmitted(string Date = null, string particular = null, string refchq = null, string widthdraw = null, string deposit = null, string bal = null, string accno = null, string type = null,string cns = null,string narr=null)
        {


            DateTime Bankdate = DateTime.ParseExact(Date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            // bool iscredit = false;
            string tranno = string.Empty;
            string errmst = string.Empty;
            string instid = _accoutdbmodel.ImprestMasters.Where(m => m.BankAccountNo == accno.Trim()).Select(m => m.InstituteId).FirstOrDefault();
            if (!string.IsNullOrEmpty(instid))
            {
                createTran(instid.Trim(), User.Identity.Name, out tranno, out errmst);
            }

            Transaction tt = _accoutdbmodel.Transaction.FirstOrDefault(m => m.TransNO == tranno.Trim());
            tt.bankDate = Bankdate;
            tt.BankPart = particular;
            tt.ChequeNO = refchq;
            //tt.Remarks = particular+" "+refchq;
            if (!string.IsNullOrEmpty(widthdraw) && !string.IsNullOrEmpty(deposit))
            {
                if (Convert.ToDecimal(widthdraw) + Convert.ToDecimal(deposit) > 0)
                {
                    tt.Amount = Convert.ToDecimal(widthdraw) + Convert.ToDecimal(deposit);
                    tt.voucherType = Convert.ToInt16(type);

                }
            }

            tt.TransCommited = true;
            tt.narration = narr;
            tt.BankAccountNO = accno.Trim();
            if (!string.IsNullOrEmpty(cns))
            {
                if (string.Equals("true", cns.Trim()))
                {
                    tt.CNS = true;
                }
            }

            ImprestMaster impst = _accoutdbmodel.ImprestMasters.Where(m => m.BankAccountNo == accno.Trim()).FirstOrDefault();
            if (_accoutdbmodel.VoucherTypes.Where(m => m.TypeId == tt.voucherType).Select(m => m.deposit).FirstOrDefault())
            {
                impst.Amount += (decimal)tt.Amount;
            }
            else
            {
                impst.Amount -= (decimal)tt.Amount;
            }
            _accoutdbmodel.SaveChanges();
            bool debiten = _accoutdbmodel.VoucherTypes.Where(m => m.TypeId == tt.voucherType).Select(m => m.deposit).FirstOrDefault();
            Transaction temptrans = _accoutdbmodel.Transaction.FirstOrDefault(m => m.TransNO == tranno.Trim());
            decimal? credit = (decimal)0.00;
            decimal? debit = (decimal)0.00;
            decimal? amount = (decimal)0.00;
            amount = temptrans.Amount;
            return Json(new { debit = debit, credit = credit, transno = temptrans.TransNO, amt = amount, Bankdate = Date, particular = particular, refchq = refchq, debiten = debiten.ToString() }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult submitBrs(FormCollection fm)
        {
            string json = fm["brsJson"];
            JavaScriptSerializer objJavascript = new JavaScriptSerializer();

            List<JsonModel> testModels = objJavascript.Deserialize<List<JsonModel>>(json);
            List<Transaction> TransactionList = new List<Transaction>();
            foreach (var handle in testModels)
            {
                Transaction tt = new Transaction();
                tt = _accoutdbmodel.Transaction.Where(m => m.TransNO == handle.TransactionNO.Trim()).FirstOrDefault();
                // tt.currentBal = string.IsNullOrEmpty(handle.Balance.Trim()) == true ? 0 : Convert.ToDecimal(handle.Balance.Trim());
                tt.bankDate = DateTime.ParseExact(handle.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                tt.BankPart = handle.Particulars;
                //tt.orderId = Convert.ToInt16(handle.OrderId);
                tt.brsDone = true;
                _accoutdbmodel.SaveChanges();
            }
            //DateTime stdt = DateTime.ParseExact((string)fm["stdt"], "dd/mm/yyyy", CultureInfo.InvariantCulture);
            //DateTime Endt = DateTime.ParseExact((string)fm["endt"], "dd/mm/yyyy", CultureInfo.InvariantCulture);
            //string Transaction = testModels.Select(m=>m.TransactionNO).FirstOrDefault();
            //string accountno = _accoutdbmodel.Transaction.Where(m => m.TransNO == Transaction.Trim()).Select(m=>m.BankAccountNO).FirstOrDefault();
            //List<Transaction> alltt= _accoutdbmodel.Transaction.Where(m => m.BankAccountNO == accountno && (m.bankDate.Value.Month == stdt.Month && m.bankDate.Value.Year == stdt.Year)).ToList();
            //var inf = (from tran in alltt
            //           join vou in _accoutdbmodel.VoucherTypes on tran.voucherType equals vou.TypeId
            //           select new TransModel()
            //           {
            //               Amount = tran.Amount,
            //               credit = vou.recoup
            //           }).ToList();
            //decimal credit = (decimal)inf.Where(m => m.credit == true).Sum(m => m.Amount);
            //decimal debit = (decimal)inf.Where(m => m.credit == false).Sum(m => m.Amount);
            //decimal finalbal;
            //ImprestMaster master=_accoutdbmodel.ImprestMasters.FirstOrDefault(m=>m.BankAccountNo== accountno.Trim());
            //master.Amount += credit;
            //master.Amount -= debit;
            //finalbal = master.Amount;
            //_accoutdbmodel.SaveChanges();
            //MonthlyBalance mb = new MonthlyBalance();
            //mb.Accno = accountno;
            //mb.Balance = finalbal;
            //mb.startDate = stdt;
            //mb.endDate = Endt;
            //mb.bankBalance = Convert.ToDecimal(fm["clbl"]);
            //_accoutdbmodel.monthlyBalance.Add(mb);
            //_accoutdbmodel.SaveChanges();
            ViewData["HeaderMsg"] = "Brs Done";
            return View("Close");
        }
        [HttpPost]
        public ActionResult importBankTxt(brsMdl bbr)
        {
            //HttpPostedFileBase file=new HttpPostedFileBase ;

            var file = Request.Files["ImportAccountDetails"];

            //var file = Request.Files["ImportAccountDetails"];
            if (file != null)
            {
                if (string.Equals(file.ContentType, "text/plain"))
                {
                    string accoutno = string.Empty;
                    string stDate = string.Empty;
                    string endDate = string.Empty;
                    bool ishastransaction = false;
                    bool gotaccountno = false;
                    bool gotStatementMonth = false;
                    List<TransModel> BrsTrans = new List<TransModel>();
                    //Session["bankstatement"] = file.InputStream;
                    var processedlist = processImportedFile(file.InputStream, out accoutno, out stDate, out endDate, out ishastransaction, out gotaccountno, out gotStatementMonth);
                    TempData["CloseBal"] = processedlist.Select(m => m.currentBal).LastOrDefault();
                    TempData["AccountOpen"] = _accoutdbmodel.ImprestMasters.Where(m => m.BankAccountNo == accoutno.Trim()).Select(m => m.Amount).FirstOrDefault();
                    TempData["accoutno"] = accoutno;

                    if (ishastransaction)
                    {
                        if (ishastransaction && gotStatementMonth)
                        {
                            DateTime dt = DateTime.ParseExact(stDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            DateTime edt = DateTime.ParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                            TempData["startDate"] = dt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                            TempData["endDate"] = edt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                            //dt.Month;
                            //var dbTransaction=_accoutdbmodel.Transaction.Where(t => t.BankAccountNO == accoutno.Trim() && t.bankDate.Value.Month == dt.Month).ToList();
                            List<TransModel> dbTransaction = (from trans in _accoutdbmodel.Transaction
                                                 join vouch in _accoutdbmodel.VoucherTypes on trans.voucherType equals vouch.TypeId
                                                 join sup in _accoutdbmodel.supplier on trans.supplier equals sup.Id
                                                 where trans.BankAccountNO == accoutno.Trim() && trans.deleted == false && trans.TransCommited == true && trans.brsDone == false && trans.bankDate.Value.Month == dt.Month && trans.bankDate.Value.Year == dt.Year
                                                 select new TransModel()
                                                 {
                                                     Amount = trans.Amount,
                                                     bankDate = trans.bankDate,
                                                     ChequeNO = trans.ChequeNO,
                                                     CommitmentNO = trans.CommitmentNO,
                                                     voucherTypeStr = vouch.VoucherTypeName,
                                                     voucherType = trans.voucherType,
                                                     voucherNo = trans.voucherNo,
                                                     TransNO = trans.TransNO,
                                                     CNS = trans.CNS,
                                                     Supplier = sup.Name,
                                                     credit = vouch.deposit,
                                                     brsEntry = vouch.Brs,
                                                     bankPart = trans.BankPart
                                                 }).ToList();
                            List<string> transno=_accoutdbmodel.cheqnotsub.Where(m => m.Accno == accoutno.Trim()).Select(m=>m.Tranno).ToList();

                            List<TransModel> chnotsubmitted = (from trans in _accoutdbmodel.Transaction
                                                              join vouch in _accoutdbmodel.VoucherTypes on trans.voucherType equals vouch.TypeId
                                                              join sup in _accoutdbmodel.supplier on trans.supplier equals sup.Id
                                                              where transno.Contains(trans.TransNO)
                                                               select new TransModel()
                                                              {
                                                                  Amount = trans.Amount,
                                                                  bankDate = trans.bankDate,
                                                                  ChequeNO = trans.ChequeNO,
                                                                  CommitmentNO = trans.CommitmentNO,
                                                                  voucherTypeStr = vouch.VoucherTypeName,
                                                                  voucherType = trans.voucherType,
                                                                  voucherNo = trans.voucherNo,
                                                                  TransNO = trans.TransNO,
                                                                  CNS = trans.CNS,
                                                                  Supplier = sup.Name,
                                                                  credit = vouch.deposit,
                                                                  brsEntry = vouch.Brs,
                                                                  bankPart = trans.BankPart
                                                              }).ToList();
                            dbTransaction.AddRange(chnotsubmitted);

                            if (dbTransaction != null)
                            {
                                List<TransModel> singletran = dbTransaction.OrderBy(m => m.bankDate).ToList();
                                TransModel newsingle = new TransModel();
                                foreach (var data in processedlist)
                                {
                                    BrsTrans.Add(data);
                                    //dbTransaction = dbTransaction.OrderBy(m=>m.Amount).ToList();
                                    if (data.DEPOSITS != null)
                                    {
                                        if (data.DEPOSITS == data.currentBal)
                                        {
                                            TempData["openBal"] = Convert.ToString(data.DEPOSITS);
                                            //break;
                                        }
                                        if (data.DEPOSITS != 0)
                                        {
                                            List<TransModel> templist = singletran.Where(m => m.credit == true).ToList();
                                            newsingle = templist.FirstOrDefault(m => m.Amount == data.DEPOSITS);
                                        }
                                    }
                                    if (data.WITHDRAWALS != null)
                                    {
                                        if (data.WITHDRAWALS != 0)
                                        {
                                            List<TransModel> templist = singletran.Where(m => m.credit == false).ToList();
                                            newsingle = templist.FirstOrDefault(m => m.Amount == data.WITHDRAWALS);
                                        }
                                    }
                                    if (newsingle != null)
                                    {
                                        int index = singletran.IndexOf(newsingle);
                                        BrsTrans.Add(newsingle);
                                        singletran.RemoveAt(index);
                                    }
                                    else
                                    {

                                    }

                                }
                                BrsTrans.AddRange(singletran);
                            }
                            return View("BrsResult", new brsMdl() { brasBankTran = BrsTrans, VoucherType = _accoutdbmodel.VoucherTypes.ToList() });
                        }
                    }

                }

            }
            return null;
        }

        public ActionResult searchaccount(brsMdl bbs)
        {
            return null;
        }

        public List<TransModel> processImportedFile(Stream filestream, out string acno, out string stmont, out string endmn, out bool hastran, out bool gotacno, out bool hasmon)
        {
            string accoutno = string.Empty;
            string stDate = string.Empty;
            string endDate = string.Empty;
            bool ishastransaction = false;
            bool gotaccountno = false;
            bool gotStatementMonth = false;


            List<transactionDetails> transaction = _importtxt.processTransList(_importtxt.ImportAccountDetails(filestream, out accoutno, out stDate, out endDate));
            TransModel tt = new TransModel();
            List<TransModel> transaclist = new List<TransModel>();
            foreach (var da in transaction)
            {
                tt = new TransModel();
                tt.bankDate = DateTime.ParseExact(da.ValueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                tt.desc = da.description;
                //tt.DEPOSITS = string.IsNullOrEmpty(da.ValueDate)==true?Convert.ToDecimal("956.56"):88;
                tt.DEPOSITS = string.IsNullOrEmpty(da.deposite.Trim()) == true ? null : (decimal?)Convert.ToDecimal(da.deposite);
                tt.WITHDRAWALS = string.IsNullOrEmpty(da.withdrawals.Trim()) == true ? null : (decimal?)Convert.ToDecimal(da.withdrawals);
                tt.ChequeNO = da.refchqno;
                tt.currentBal = string.IsNullOrEmpty(da.balance) == true ? null : (decimal?)Convert.ToDecimal(da.balance);
                tt.imported = true;
                tt.BankAccountNO = accoutno;
                if (tt.DEPOSITS != 0)
                {
                    tt.credit = false;
                }
                else
                {
                    tt.credit = true;
                }
                //tt.Amount = tt.DEPOSITS +  tt.WITHDRAWALS;
                //string.IsNullOrEmpty(da.deposite) == true ? null : 55;
                //tt.WITHDRAWALS = string.IsNullOrEmpty(da.withdrawals)==true? null :Convert.ToDecimal(da.withdrawals);
                transaclist.Add(tt);
            }
            if (transaclist != null)
            {
                if (transaclist.Count > 0)
                {
                    if (!string.IsNullOrEmpty(accoutno))
                    {
                        if (accoutno.Length == 13)
                            gotaccountno = true;
                    }
                    if (!string.IsNullOrEmpty(stDate))
                    {
                        try
                        {
                            DateTime dt = DateTime.ParseExact(stDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            gotStatementMonth = true;
                        }
                        catch
                        {
                            gotStatementMonth = false;
                        }
                    }
                    ishastransaction = true;
                }
            }
            acno = accoutno;
            stmont = stDate;
            endmn = endDate;
            hastran = ishastransaction;
            gotacno = gotaccountno;
            hasmon = gotStatementMonth;
            return transaclist;
        }

        [NonAction]
        private bool createTran(string instid, string usrmail, out string TransactonNo, out string ermsgg)
        {
            string AccountNO = _accoutdbmodel.ImprestMasters.Where(m => m.InstituteId == instid.Trim()).Select(m => m.BankAccountNo).FirstOrDefault();
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
                _accoutdbmodel.Database.ExecuteSqlCommand("exec createTran @instid,@UserEmail,@BankAccountNO,@TransactionNo OUT,@isFailed OUT,@ermsg OUT", sInstid, usrmailP, Accno, genTrans, Isfailed, ermsg);
                _accoutdbmodel.Database.Connection.Close();
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
    }
}