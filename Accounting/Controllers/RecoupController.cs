using Accounting.data.Database;
using Accounting.Models.Recoup;
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
    public class RecoupController : Controller
    {
        // GET: Recoup
        readonly IAccountingDbModel _accountdb;
        public RecoupController(IAccountingDbModel accountdb)
        {
            _accountdb = accountdb;
        }

        public ActionResult Index(string Accno, string vt)
        {
            //_accountdb.Transaction.Where(trans.BankAccountNO == mdl.AccountNO.Trim() && m.deleted==false && m.brsDone==false && m.RecupDone==false && m.voucherType!=1 && m.voucherType != 4 && m.voucherType != 8).AsQueryable();
            List<Transaction> TransList = _accountdb.Transaction.Where(m => m.BankAccountNO == Accno && m.RecupDone == false).ToList();
            IEnumerable<TransModel> TransModels = new List<TransModel>();
            if (string.Equals(vt.Trim(), "1"))
            {
                TransModels = (from trans in TransList
                               join vouchtype in _accountdb.VoucherTypes on trans.voucherType equals vouchtype.TypeId
                               where trans.voucherType != 1 && trans.voucherType != 4 && trans.voucherType < 9
                               select new TransModel()
                               {
                                   Amount = trans.Amount,
                                   ChequeNO = trans.ChequeNO,
                                   CommitmentNO = trans.CommitmentNO,
                                   voucherNo = trans.voucherNo,
                                   voucherTypeStr = vouchtype.VoucherTypeName,
                                   ProjectNo = trans.ProjectNo,
                                   bankDate = trans.bankDate,
                                   TransNO = trans.TransNO
                               }).ToList();
            }
            if (string.Equals(vt.Trim(), "5"))
            {
                TransModels = (from trans in TransList
                               join vouchtype in _accountdb.VoucherTypes on trans.voucherType equals vouchtype.TypeId
                               where trans.voucherType == 4
                               select new TransModel()
                               {
                                   Amount = trans.Amount,
                                   ChequeNO = trans.ChequeNO,
                                   CommitmentNO = trans.CommitmentNO,
                                   voucherNo = trans.voucherNo,
                                   voucherTypeStr = vouchtype.VoucherTypeName,
                                   ProjectNo = trans.ProjectNo,
                                   bankDate = trans.bankDate,
                                   TransNO = trans.TransNO
                               }).ToList();
            }

            return PartialView("RecoupSelect", TransModels);
        }
        [HttpPost]
        public JsonResult SubmitRecup(FormCollection fm)


        {
            string jsonFile = fm["ToJson"];
            string Transaction = fm["TransNORE"];
            string bankstr = fm["BkDateRE"];
            string voucherNO = fm["voucherNoRE"];
            string remarks = fm["RemarksRE"];
            string narration = fm["NarrationRE"];

            JavaScriptSerializer objJavascript = new JavaScriptSerializer();
            try
            {
                // var testTran = _accountdb.Transaction.Where(m => m.TransNO == Transaction).FirstOrDefault();

                List<string> testModels = objJavascript.Deserialize<List<string>>(jsonFile);
                if (testModels.Count > 0)
                {
                    if (_accountdb.Transaction.Where(m => m.TransNO == Transaction.Trim()).FirstOrDefault() != null)
                    {
                        string accountNo = _accountdb.Transaction.Where(m => m.TransNO == testModels.FirstOrDefault().Trim()).Select(m => m.BankAccountNO).FirstOrDefault();
                        //var recuipid = _accountdb.Database.SqlQuery<string>("select dbo.GENERATERECOUPID({0})", accountNo);
                        //string getnid = recuipid.ToList().FirstOrDefault();
                        string getnid = string.Empty;
                        string errmsg = string.Empty;
                        decimal totalamount = 0;
                        if (!CreateRecId(accountNo, User.Identity.Name, out getnid, out errmsg))
                        {
                            foreach (string transno in testModels)
                            {
                                Transaction TempTr = _accountdb.Transaction.Where(m => m.TransNO == transno.Trim()).FirstOrDefault();
                                TempTr.Recoupid = getnid;
                                TempTr.RecupDone = true;
                                _accountdb.SaveChanges();
                                totalamount += Convert.ToDecimal(TempTr.Amount);
                            }
                            Transaction RecuipT = _accountdb.Transaction.FirstOrDefault(m => m.TransNO == Transaction.Trim());
                            RecuipT.Amount = totalamount;
                            RecuipT.Recoupid = getnid;
                            RecuipT.RecupDone = true;
                            RecuipT.TransCommited = true;
                            RecuipT.narration = narration;
                            RecuipT.Remarks = remarks;
                            RecuipT.voucherNo = voucherNO;
                            RecuipT.bankDate = DateTime.ParseExact(bankstr, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            RecuipT.voucherType = 1;

                            RecuipT.BankAccountNO = accountNo.Trim();
                            ImprestMaster mstr= _accountdb.ImprestMasters.Where(m => m.BankAccountNo == RecuipT.BankAccountNO).FirstOrDefault();
                            mstr.Amount += (decimal)totalamount;
                            
                            //_accountdb.Transaction.Add(RecuipT);
                            _accountdb.SaveChanges();
                        }
                        else
                        {
                            return Json(new { job = "f" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    return Json(new { job = "s" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { job = "ns" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(new { job = "f" }, JsonRequestBehavior.AllowGet);
            }

        }

        [NonAction]
        private bool CreateRecId(string AccountNo, string usrmail, out string GeneratedId, out string ermsgg)
        {
            SqlParameter AccountNO = new SqlParameter
            {
                ParameterName = "@accno",
                Value = AccountNo,
                SqlDbType = SqlDbType.NVarChar,
                Size = 14
            };
            SqlParameter usrmailP = new SqlParameter
            {
                ParameterName = "@UserEmail",
                SqlDbType = SqlDbType.NVarChar,
                Size = 80,
                Value = usrmail
            };

            SqlParameter genid = new SqlParameter
            {
                ParameterName = "@recoupid",
                SqlDbType = SqlDbType.NVarChar,
                Direction = ParameterDirection.Output,
                Size = 33
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
                _accountdb.Database.ExecuteSqlCommand("exec createRecoupTran @accno,@UserEmail,@recoupid OUT,@isFailed OUT,@ermsg OUT", AccountNO, usrmailP, genid, Isfailed, ermsg);
                _accountdb.Database.Connection.Close();
                GeneratedId = Convert.ToString(genid.Value);
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
                GeneratedId = "none";
                ermsgg = "AppCatch";
                return false;
            }

        }

        [NonAction]
        private bool createTran(string instid, string usrmail, out string TransactonNo, out string ermsgg)
        {
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
                _accountdb.Database.ExecuteSqlCommand("exec createTran @instid,@UserEmail,@TransactionNo OUT,@isFailed OUT,@ermsg OUT", sInstid, usrmailP, genTrans, Isfailed, ermsg);
                _accountdb.Database.Connection.Close();
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