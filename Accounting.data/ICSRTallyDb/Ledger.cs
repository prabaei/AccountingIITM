//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Accounting.data.ICSRTallyDb
{
    using System;
    using System.Collections.Generic;
    
    public partial class Ledger
    {
        public int Autoid { get; set; }
        public Nullable<decimal> TallyMasterid { get; set; }
        public string Name { get; set; }
        public string Parent { get; set; }
        public string CrDr { get; set; }
        public Nullable<decimal> OpeningBalance { get; set; }
        public string @void { get; set; }
        public Nullable<System.DateTime> LedDate { get; set; }
    }
}
