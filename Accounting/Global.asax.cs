using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Accounting.data.Database;

using Accounting.data.Database.FoxOffice;
using Accounting.data.Database.FACCT;
using System.Web.Security;
using System.Security.Principal;
using Accounting.App_Start;
using System.Web.Optimization;
using Accounting.data.Database.PCFACCT;
using System.Web.Configuration;
using Accounting.data.services.Logger;
using Accounting.data.services.Export;
using Accounting.data.services.Import;
using System.IO;
using Accounting.data.ICSRTallyDb;

namespace Accounting
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {


            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            // builder.RegisterType<AccountingDbModel>().InstancePerRequest();

            builder.RegisterType<AccountingDbModel>().As<IAccountingDbModel>();
            builder.RegisterType<FoxOfficeService>().As<IfoxOfficeService>();
            builder.RegisterType<FacctService>().As<IFacctService>();
            builder.RegisterType<PCFACCTService>().As<IPCFACCTService>();
            builder.RegisterType<ExceptionLogger>().As<IExceptionLogger>();
            builder.RegisterType<ExportToExcel>().As<IExportToExcel>();
            builder.RegisterType<ImportExcel>().As<IImportExcel>();
            builder.RegisterType<ImportTxt>().As<IImportTxt>();
            builder.RegisterType<ICSRDBTALLYEntities>().As<IICSRDBTALLYEntities>();

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            AreaRegistration.RegisterAllAreas();
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //using (AccountingDbModel accdbmdl = new AccountingDbModel())
            //{
            //    accdbmdl.recoupRecord.FirstOrDefault();
            //    if (string.IsNullOrEmpty(File.ReadAllText(Server.MapPath("~/SqlFiles/database.txt"))))
            //    {
            //        string script = File.ReadAllText(Server.MapPath("~/SqlFiles/country.sql"));
            //        accdbmdl.Database.ExecuteSqlCommand(script);
            //        script = File.ReadAllText(Server.MapPath("~/SqlFiles/commitTran.sql"));
            //        accdbmdl.Database.ExecuteSqlCommand(script);
            //        script = File.ReadAllText(Server.MapPath("~/SqlFiles/createTran.sql"));
            //        accdbmdl.Database.ExecuteSqlCommand(script);
            //        script = File.ReadAllText(Server.MapPath("~/SqlFiles/GENERATETRANSID.sql"));
            //        accdbmdl.Database.ExecuteSqlCommand(script);
            //        script = File.ReadAllText(Server.MapPath("~/SqlFiles/InsertUserTbl.sql"));
            //        accdbmdl.Database.ExecuteSqlCommand(script);
            //        script = File.ReadAllText(Server.MapPath("~/SqlFiles/RESETPASSWORD.sql"));
            //        accdbmdl.Database.ExecuteSqlCommand(script);
            //        script = File.ReadAllText(Server.MapPath("~/SqlFiles/verifyUser.sql"));
            //        accdbmdl.Database.ExecuteSqlCommand(script);
            //        script = File.ReadAllText(Server.MapPath("~/SqlFiles/projectType.sql"));
            //        accdbmdl.Database.ExecuteSqlCommand(script);
            //        script = File.ReadAllText(Server.MapPath("~/SqlFiles/createAdmin.sql"));
            //        accdbmdl.Database.ExecuteSqlCommand(script);
            //        script = File.ReadAllText(Server.MapPath("~/SqlFiles/voucherType.sql"));
            //        accdbmdl.Database.ExecuteSqlCommand(script);
            //        script = File.ReadAllText(Server.MapPath("~/SqlFiles/acctran_delete_default.sql"));
            //        accdbmdl.Database.ExecuteSqlCommand(script);
            //        script = File.ReadAllText(Server.MapPath("~/SqlFiles/createRecoupTran.sql"));
            //        accdbmdl.Database.ExecuteSqlCommand(script);
            //        script = File.ReadAllText(Server.MapPath("~/SqlFiles/GENERATERECOUPID.sql"));
            //        accdbmdl.Database.ExecuteSqlCommand(script);
            //        accdbmdl.SaveChanges();
            //        File.WriteAllText(Server.MapPath("~/SqlFiles/database.txt"), "Database installed");
            //    }
            //}

        }
        protected void Session_Start()
        {
            HttpContext.Current.Session["sessid"] = Guid.NewGuid().ToString();
        }
        protected void Application_BeginRequest()
        {
            string url = HttpContext.Current.Request.RawUrl;
            var iii = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host +
(Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
        }
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = HttpContext.Current.Server.GetLastError();
            using (AccountingDbModel accdbmdl = new AccountingDbModel())
            {
                ErrorLog er = new ErrorLog();
                er.Error = ex.Message;
                er.LoggedAt = DateTime.Now;
                er.UserName = User.Identity.Name;
                accdbmdl.ErrorLog.Add(er);
                int done = accdbmdl.SaveChanges();
                //Session["errorLogged"] = true;
                //if(done!=1)
                //{
                //    StreamWriter sw = new StreamWriter(Server.MapPath("~/ErrorLogger/errlog.txt"));
                //    sw.WriteLine();
                //    sw.WriteLine();
                //    sw.WriteLine();
                //    sw.WriteLine("------------"+DateTime.Now.ToString()+ "------------");
                //    sw.WriteLine("Error : " + ex.Message);
                //    sw.WriteLine("UserName : " + User.Identity.Name);
                //    sw.Close();
                //}
                //string pageNotFound = string.Empty;
                //if (HttpContext.Current.Request.IsSecureConnection)
                //{
                //    pageNotFound = string.Format("https://{0}/StaticContent/ErrorPage", HttpContext.Current.Request.Url.Authority);

                //}
                //else
                //{
                //    pageNotFound = string.Format("http://{0}/StaticContent/ErrorPage", HttpContext.Current.Request.Url.Authority);

                //}
                //HttpContext.Current.Response.Redirect(pageNotFound);
                // if (pageNotFound != null)
                // Response.Redirect(pageNotFound);

            }


        }
        protected void Application_EndRequest(Object sender, EventArgs e)
        {
            int ex = HttpContext.Current.Response.StatusCode;
            if (ex == 404)
            {
                string pageNotFound = string.Empty;
                if (HttpContext.Current.Request.IsSecureConnection)
                {
                    pageNotFound = string.Format("https://{0}/StaticContent/PageNotFound", HttpContext.Current.Request.Url.Authority);

                }
                else
                {
                    pageNotFound = string.Format("http://{0}/StaticContent/PageNotFound", HttpContext.Current.Request.Url.Authority);

                }
                if (pageNotFound != null)
                    Response.Redirect(pageNotFound);
            }
            //// filter out 404 responses
            //// var httpException = ex as HttpException;

            //var url = HttpContext.Current.Request.Url;
            //var urlpre = HttpContext.Current.Request.UrlReferrer;

            //
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                if (!authTicket.Expired | authTicket.IsPersistent)
                {
                    string[] data = authTicket.UserData.Split(new char[] { '|' });
                    if (data.Length == 1)
                    {
                        data = new string[] { data[0], string.Empty };
                    }
                    using (AccountingDbModel accdbmdl = new AccountingDbModel())
                    {
                        var usersessid = accdbmdl.UserSess.FirstOrDefault(m => m.userEmail == authTicket.Name);
                        if (usersessid != null)
                        {
                            if (string.Equals(usersessid.sessionId, data[1]))
                            {
                                string[] roles = new string[] { data[0] };

                                // Create the IIdentity instance
                                IIdentity id = new GenericIdentity(authTicket.Name, "Forms");

                                // Create the IPrinciple instance
                                IPrincipal principal = new GenericPrincipal(id, roles);


                                Context.User = principal;
                                FormsAuthenticationTicket ticket1 =
                           new FormsAuthenticationTicket(
                                1,                                   // version
                                Context.User.Identity.Name,   // get username  from the form
                                DateTime.Now,                        // issue time is now
                                DateTime.Now.AddMinutes(10),        // expires in 10 minutes
                                authTicket.IsPersistent,      // cookie is not persistent
                                authTicket.UserData                              // role assignment is stored
                                                                                 // in userData
                                );
                                HttpCookie cookie1 = new HttpCookie(
                          FormsAuthentication.FormsCookieName,
                          FormsAuthentication.Encrypt(ticket1));
                                Response.Cookies.Add(cookie1);
                            }
                        }

                    }


                    //FormsAuthentication.SetAuthCookie(Context.User.Identity.Name, authTicket.IsPersistent);
                }

            }
        }
    }
}
