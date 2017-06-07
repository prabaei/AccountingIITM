using Accounting.data.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.data.services.Logger
{
    public class ExceptionLogger: IExceptionLogger
    {
        public int logError(IAccountingDbModel accdb,string ErrorMsg, string Username)
        {
            ErrorLog er = new ErrorLog() { Error=ErrorMsg,LoggedAt=DateTime.Now,UserName=Username};
            accdb.ErrorLog.Add(er);
            return accdb.SaveChanges();
        }
    }
}
