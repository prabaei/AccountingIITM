using Accounting.data.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Accounting.Models.BRS
{
    public class brsMdl
    {
        public string accountno { get; set; }
        public string TransactionMonth { get; set; }
        
        public List<SelectListItem> dropdownlist { get; set; }

        public List<TransModel> brasBankTran { get; set; }

        public List<VoucherType> VoucherType { get; set; }
    }
}