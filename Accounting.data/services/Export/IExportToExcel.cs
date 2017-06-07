using Accounting.data.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.data.services.Export
{
    public interface IExportToExcel
    {
        byte[] ExportAccountDetails(IEnumerable<ImprestMaster> accdetail);
        byte[] ExportTransaction(AccountTrans transdetail);

        byte[] ExportClimbNS(AccountTrans transdetail);
    }
}
