using Accounting.AccountAuthorize;
using Accounting.data.Database;
using Accounting.data.services.Logger;
using Accounting.Models.Login;
using Accounting.Models.staff;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;

namespace Accounting.Controllers
{
    public class StaffController : Controller
    {
        private readonly IAccountingDbModel _AccountDb;
        

        public StaffController(IAccountingDbModel AccountDb)
        {
            _AccountDb = AccountDb;
            //_AccountDb.UserTbl.FirstOrDefault();
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

        // GET: Staff
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }
        public ActionResult Login(string Email=null)
        {

            //requesturl = requesturl == null ? Url.Action("Index","http://localhost:2432/Home" ) : requesturl;
            //if(requesturl==null)
            //{
            //    return RedirectToAction("Index","ImprestMaster");
            //}
            //else
            //{
            //    TempData["returnurl"] = TempData["returnurl"] == null ? new Uri(requesturl) : (Uri)TempData["returnurl"];
            if (Email != null)
                return View(new LoginMdl() { username=Email});
                return View();
            //}
            
        }
        [HttpPost]
        public JsonResult Login(LoginMdl loginDetail)
        {
            string ermsg = string.Empty;
            string userdata="default";
            int role = 0;
         
            if(verifyUser(loginDetail.username, loginDetail.password==null?"*":loginDetail.password, out ermsg,out role))
            {
                
                switch (role)
                {
                    case 1:
                         userdata = "staff";
                        break;
                    case 2:
                         userdata = "admin";
                        break;
                }
                UserSession user = _AccountDb.UserSess.FirstOrDefault(m => m.userEmail == loginDetail.username);
                if (user != null)
                {
                    user.sessionId = (string)Session["sessid"];
                    _AccountDb.SaveChanges();
                }
                else
                {
                    UserSession Usersessiondata = new UserSession() { userEmail = loginDetail.username, sessionId = (string)Session["sessid"] };
                    _AccountDb.UserSess.Add(Usersessiondata);
                    _AccountDb.SaveChanges();

                }

                userdata = userdata + "|" + (string)Session["sessid"];
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                    1,
                    loginDetail.username,
                    DateTime.Now,
                    DateTime.Now.AddDays(30),
                    loginDetail.rememberme,
                    userdata,
                    FormsAuthentication.FormsCookieName
                    );
                string encticket = FormsAuthentication.Encrypt(ticket);
                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encticket);
                cookie.HttpOnly = true;
                
                Response.Cookies.Add(cookie);
                return Json(new
                {
                    validuser = Convert.ToString(true),
                    errmsg = ermsg
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                switch(ermsg)
                {
                    case "RSTPAS":
                        return Json(new
                        {
                            validuser = Convert.ToString(false),
                            errmsg = "resetPassword",
                            email= loginDetail.username
                        }, JsonRequestBehavior.AllowGet);
                        
                    case "NCRCD":
                        return Json(new
                        {
                            validuser = Convert.ToString(false),
                            errmsg = "mail id not registered"
                        }, JsonRequestBehavior.AllowGet);
                        
                    case "NTVLDUS":
                        return Json(new
                        {
                            validuser = Convert.ToString(false),
                            errmsg = "wrong password"
                        }, JsonRequestBehavior.AllowGet);
                    default:
                        return Json(new
                        {
                            validuser = Convert.ToString(false),
                            errmsg = "contact computer section"
                        }, JsonRequestBehavior.AllowGet);
                }
            }
            //var a = FormsAuthentication.GetRedirectUrl(loginDetail.username, false);
            //Response.Redirect(FormsAuthentication.GetRedirectUrl(loginDetail.username, false));
        }
        //[HttpPost]
        public ActionResult RedirectAction(string ReturnUrl=null)
        {
            if (!string.IsNullOrEmpty(ReturnUrl))
                Response.Redirect(ReturnUrl);
            else
                Response.Redirect(Url.Action("Index", "Home", new {  area=""}));
            return null;
        }
        public ActionResult logout()
        {
            FormsAuthentication.SignOut();
            // clear authentication cookie
            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);

            // clear session cookie (not necessary for your current problem but i would recommend you do it anyway)
            SessionStateSection sessionStateSection = (SessionStateSection)WebConfigurationManager.GetSection("system.web/sessionState");
            HttpCookie cookie2 = new HttpCookie(sessionStateSection.CookieName, "");
            cookie2.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie2);
            return RedirectToAction("Login");
        }
        //[AccountAutherize(Roles = "admin")]
        //public ActionResult Registration()
        //{

        //    return View();
        //}
        //[HttpPost]
        //public JsonResult Registration(Register regmdl)
        //{
        //    bool iscommited;
        //    string errmsg;
        //    InsertUserTbl(regmdl.Email, regmdl.Name, regmdl.Gender, regmdl.password,regmdl.UsrRole, out iscommited, out errmsg);
        //    return Json(new
        //    {
        //        iscommited = Convert.ToString(iscommited),
        //        errmsg = errmsg
        //    }, JsonRequestBehavior.AllowGet);
        //}
        //[NonAction]
        //public bool InsertUserTbl(string email,string name,int gender,string userpassword,int userrole,out bool iscommited,out string errmsg) {
          
        //   SqlParameter Email = new SqlParameter
        //   {
        //       ParameterName = "@Email",
        //       Value = email,
        //       Size = -1,
        //    };
        //    SqlParameter Name = new SqlParameter
        //    {
        //        ParameterName = "@Name",
        //        SqlDbType = SqlDbType.NVarChar,
        //        Value= name,
        //        Size = -1
        //    };
        //    SqlParameter Gender = new SqlParameter
        //    {
        //        ParameterName = "@Gender",
        //        SqlDbType = SqlDbType.Int,
        //        Value= gender

        //    };
            
        //    SqlParameter UserRole = new SqlParameter
        //    {
        //        ParameterName = "@UserRol",
        //        SqlDbType = SqlDbType.Int,
        //        Value = userrole

        //    };
        //    SqlParameter UserPassword = new SqlParameter
        //    {
        //        ParameterName = "@UserPassword",
        //        SqlDbType = SqlDbType.NVarChar,
        //        Size = 1000,
        //        Value= userpassword
        //    };
        //    SqlParameter IsCommit = new SqlParameter
        //    {
        //        ParameterName = "@IsCommit",
        //        SqlDbType = SqlDbType.Bit,
        //        Direction = ParameterDirection.Output,
        //    };
        //    SqlParameter erMsg = new SqlParameter
        //    {
        //        ParameterName = "@erMsg",
        //        SqlDbType = SqlDbType.NVarChar,
        //        Direction=ParameterDirection.Output,
        //        Size = 10
        //    };
        //    _AccountDb.Database.ExecuteSqlCommand("exec InsertUserTbl @Email,@Name,@Gender,@UserPassword,@UserRol,@IsCommit OUT,@erMsg OUT", Email, Name, Gender, UserPassword, UserRole, IsCommit, erMsg);
        //    _AccountDb.Database.Connection.Close();
        //    errmsg = Convert.ToString(erMsg.Value == DBNull.Value ? "" : erMsg.Value);
        //    iscommited = IsCommit.Value == DBNull.Value ? false : (bool)IsCommit.Value;
        //    return iscommited;
        //}

        public ActionResult ResetPassword(string curusr=null)
        {
            if (string.Equals(curusr, "yes"))
            {
                Response.Redirect(Url.Action("ResetPassword","Staff", new { admin="",Email= User.Identity.Name }));
                
            }
            return View();
        }
        [HttpPost]
        public ActionResult ResetPassword(string Email,FormCollection rstmdl)
        {
            string errmsg = string.Empty;
            if(Resetpassword(Email, rstmdl["userpassword"], out errmsg))
            {
                logout();
                Response.Redirect(Url.Action("Login","Staff", new { Email=Email, area =""}));
            }
            else
            {

            }
            return View();
        }
        [NonAction]
        public bool Resetpassword(string email,string password,out string errmsg)
        {
            SqlParameter Email = new SqlParameter
            {
                ParameterName = "@Email",
                Value = email,
                Size = 80,
            };
            SqlParameter Password = new SqlParameter
            {
                ParameterName = "@Password",
                SqlDbType = SqlDbType.VarChar,
                Value = password,
                Size = -1
            };

            SqlParameter updatedone = new SqlParameter
            {
                ParameterName = "@Updatedone",
                SqlDbType = SqlDbType.Bit,
                Direction = ParameterDirection.Output,
                
            };
            SqlParameter Errmsg = new SqlParameter
            {
                ParameterName = "@Errmsg",
                SqlDbType = SqlDbType.NVarChar,
                Direction = ParameterDirection.Output,
                Size = 10
            };
            _AccountDb.Database.ExecuteSqlCommand("EXEC RESETPASSWORD @Email,@Password,@Updatedone out,@Errmsg out", Email, Password, updatedone, Errmsg);
            errmsg = Convert.ToString(Errmsg.Value == DBNull.Value ? "" : Errmsg.Value);
            return updatedone.Value == DBNull.Value ? false : (bool)updatedone.Value;
        }

        [NonAction]
        public bool verifyUser(string email,string password,out string ermsg,out int role)
        {
            SqlParameter Email = new SqlParameter
            {
                ParameterName = "@Email",
                Value = email,
                Size = -1,
            };
            SqlParameter Password = new SqlParameter
            {
                ParameterName = "@Password",
                SqlDbType = SqlDbType.VarChar,
                Value = password,
                Size = 30
            };
            SqlParameter valid = new SqlParameter
            {
                ParameterName = "@valid",
                SqlDbType = SqlDbType.Bit,
                Direction = ParameterDirection.Output,
            };
            SqlParameter erMsg = new SqlParameter
            {
                ParameterName = "@ermsg",
                SqlDbType = SqlDbType.NVarChar,
                Direction = ParameterDirection.Output,
                Size = 10
            };
            SqlParameter sqlrole = new SqlParameter
            {
                ParameterName = "@role",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output,
            };
            
            _AccountDb.Database.ExecuteSqlCommand("EXEC verifyUser @Email,@Password,@ermsg out,@valid out,@role out", Email, Password, valid, erMsg, sqlrole);
            _AccountDb.Database.Connection.Close();
            ermsg = Convert.ToString(erMsg.Value == DBNull.Value ? "" : erMsg.Value);
            role = Convert.ToInt32(sqlrole.Value==DBNull.Value?0:sqlrole.Value);
            return valid.Value == DBNull.Value ? false : (bool)valid.Value;
        }
    }
}