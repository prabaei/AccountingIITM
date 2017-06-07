using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Accounting.Controllers
{
    public class TestMailController : Controller
    {
        // GET: TestMail
        public ActionResult Index()
        {
            MailMessage mail = new MailMessage();
            mail.To.Add("prababoy123@gmail.com");
            mail.From = new MailAddress("prabaei@gmail.com");
            mail.Subject = "testmail";
            string Body = "why do we fall";
            mail.Body = Body;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential
            ("", "");// Enter seders User name and password  
            smtp.EnableSsl = true;
            smtp.Send(mail);
            return View();
        }
    }
}