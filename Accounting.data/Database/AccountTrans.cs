using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Accounting.data.Database;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Accounting.data.Database
{
    public class AccountTrans
    {
        [Display(Name = "Coordinator Name")]
        public string coordnatorName { get; set; }
        [Display(Name = "AccountNO")]
        public string AccountNO { get; set; }
        [Display(Name = "From Date")]
        public string Fromdate { get; set; }
        [Display(Name = "To Date")]
        public string ToDate { get; set; }
        [Display(Name = "Voucher Type")]
        public int VoucherTypeID { get; set; }

        public bool showdeleted { get; set; }

        public bool ShowBRS { get; set; }

        public bool cns {get;set;}


        public bool Allacc { get; set; }
        public IEnumerable<SelectListItem> vouchertypeList { get; set; }
        public IEnumerable<TransModel> accoutnTransaction {get;set;}
    }
}