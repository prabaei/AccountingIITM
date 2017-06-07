using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestProcess;

namespace Accounting.data.services.Import
{
    public interface IImportTxt
    {
        List<transactionDetails> ImportAccountDetails(Stream stream, out string AccoutnNo, out string fromdate, out string todate);
        List<transactionDetails> processTransList(List<transactionDetails> translist);
    }
}
