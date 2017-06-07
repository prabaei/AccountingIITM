using Accounting.AccountAuthorize;
using Accounting.data.Database;
using Accounting.Models.Login;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Accounting.Areas.admin.Controllers
{
    [AccountAutherize(Roles = "admin")]
    public class StaffRegController : Controller
    {
        private readonly IAccountingDbModel _AccountDb;


        public StaffRegController(IAccountingDbModel AccountDb)
        {
            this._AccountDb = AccountDb;
            //_AccountDb.UserTbl.FirstOrDefault();
        }
       
        // GET: admin/StaffReg
        public ActionResult Index()
        {
            return View();
        }
        [AccountAutherize(Roles ="admin")]
        public ActionResult Registration()
        {
            return View("Register",new Register());
        }
        [HttpPost]
        public JsonResult Registration(Register regmdl)
        {
            bool iscommited;
            string errmsg;
            InsertUserTbl(regmdl.Email, regmdl.Name, regmdl.Gender, "123", regmdl.UsrRole, out iscommited, out errmsg);
            return Json(new
            {
                iscommited = Convert.ToString(iscommited),
                errmsg = errmsg
            }, JsonRequestBehavior.AllowGet);
        }
        [NonAction]
        public bool InsertUserTbl(string email, string name, int gender, string userpassword, int userrole, out bool iscommited, out string errmsg)
        {

            SqlParameter Email = new SqlParameter
            {
                ParameterName = "@Email",
                Value = email,
                Size = -1,
            };
            SqlParameter Name = new SqlParameter
            {
                ParameterName = "@Name",
                SqlDbType = SqlDbType.NVarChar,
                Value = name,
                Size = -1
            };
            SqlParameter Gender = new SqlParameter
            {
                ParameterName = "@Gender",
                SqlDbType = SqlDbType.Int,
                Value = gender

            };

            SqlParameter UserRole = new SqlParameter
            {
                ParameterName = "@UserRol",
                SqlDbType = SqlDbType.Int,
                Value = userrole

            };
            SqlParameter UserPassword = new SqlParameter
            {
                ParameterName = "@UserPassword",
                SqlDbType = SqlDbType.NVarChar,
                Size = 1000,
                Value = userpassword
            };
            SqlParameter IsCommit = new SqlParameter
            {
                ParameterName = "@IsCommit",
                SqlDbType = SqlDbType.Bit,
                Direction = ParameterDirection.Output,
            };
            SqlParameter erMsg = new SqlParameter
            {
                ParameterName = "@erMsg",
                SqlDbType = SqlDbType.NVarChar,
                Direction = ParameterDirection.Output,
                Size = 10
            };
            _AccountDb.Database.ExecuteSqlCommand("exec InsertUserTbl @Email,@Name,@Gender,@UserPassword,@UserRol,@IsCommit OUT,@erMsg OUT", Email, Name, Gender, UserPassword, UserRole, IsCommit, erMsg);
            _AccountDb.Database.Connection.Close();
            errmsg = Convert.ToString(erMsg.Value == DBNull.Value ? "" : erMsg.Value);
            iscommited = IsCommit.Value == DBNull.Value ? false : (bool)IsCommit.Value;
            return iscommited;
        }
    }
}