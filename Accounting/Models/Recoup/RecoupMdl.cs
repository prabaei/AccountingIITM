using Accounting.data.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Accounting.Models.Recoup
{
    public class RecoupMdl
    {
        public string CName { get; set; }
        public string AccountNO { get; set; }

        public IEnumerable<TransModel> transactionList { get; set; }
    }
}