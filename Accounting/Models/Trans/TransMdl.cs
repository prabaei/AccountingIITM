using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Accounting.Models.Trans
{
    public class TransMdl
    {
        public int VoucherType { get; set; }
        public List<SelectListItem> VoucherTypeList = new List<SelectListItem>() {
            new SelectListItem() {Text="",Value="" },

        };
    }
}