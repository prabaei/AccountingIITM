using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Accounting.Models.ImprestMstr
{
    public class ImprestMstrMdl
    {
        [Display(Name = "INSTITUTE ID")]
        public string INSTID { get; set; }
        public string ProjectNO { get; set; }
        public bool gotCoordDetails { get; set; }
    }
    public class ImprestJson
    {
        public string INSTD { get; set; }
        public string ProjNo { get; set; }
        public string Name { get; set; }
        public string StartDate { get; set; }
        public string CloseDate { get; set; }
        public string AccNO { get; set; }
        public string Amt { get; set; }
        public string Limit { get; set; }
    }
}