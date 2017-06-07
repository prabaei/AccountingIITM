using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Accounting.Models.BRS
{
    public class JsonModel
    {
        public string Date { get; set; }
        public string Particulars { get; set; }
        public string VchType { get; set; }
        public string VchNo { get; set; }
        public string REFCHQNO { get; set; }
        public string Debit { get; set; }
        public string Credit { get; set; }
        public string WITHDRAWALS { get; set; }
        public string DEPOSITS { get; set; }
        public string Balance { get; set; }
        public string TransactionNO { get; set; }

        public string OrderId { get; set; }
    }
}