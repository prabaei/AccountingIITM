using System;
using System.Web;

namespace Accounting.data.services.cookie
{
    public class CookieService
    {
        public HttpCookie CreateStudentCookie()
        {
            HttpCookie TransactionCookie = new HttpCookie("TransactionCookie");
            TransactionCookie.Value = "Transaction";
            TransactionCookie.Expires = DateTime.Now.AddDays(1);
            return TransactionCookie;
        }
    }
}
