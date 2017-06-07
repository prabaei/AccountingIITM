using Accounting.data.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.data.services.Logger
{
   public interface IExceptionLogger
    {
        int logError(IAccountingDbModel accdb, string ErrorMsg, string Username);
    }
}
