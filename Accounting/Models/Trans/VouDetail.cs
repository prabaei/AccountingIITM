using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Accounting.Models.Trans
{
    public class VouDetail
    {
        public string vouno { get; set; }
        public decimal? AMOUNT { get; set; }
        public string PART { get; set; }
        public string DISC { get; set; }

        public string PONO { get; set; }

        public string CQNO { get; set; }

        public string Date { get; set; }

        public string HEAD { get; set; }

        public int ProjectType { get; set; }

        public string projectNO { get; set; }

        public string CommitNo { get; set; }

    }
}