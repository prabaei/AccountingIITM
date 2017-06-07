using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Accounting.Models.reports
{
    public class SearchCriteria
    {
        public string Name { get; set; }
        public string AccountNO { get; set; }
        public bool climbNotSub { get; set; }
        public bool allaccount { get; set; }
    }
}