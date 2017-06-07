using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Accounting.data.Database;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Accounting.Models.Trans
{
    public class AccountTrans
    {
        [Display(Name ="AccountNO")]
        public string AccountNO { get; set; }
        [Display(Name = "From Date")]
        public string Fromdate { get; set; }
        [Display(Name = "To Date")]
        public string ToDate { get; set; }
        [Display(Name = "Voucher Type")]
        public int VoucherTypeID { get; set; }

        public IEnumerable<SelectListItem> vouchertypeList { get; set; }
        public IEnumerable<TransModel> accoutnTransaction {get;set;}
    }
}