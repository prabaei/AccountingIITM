using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Accounting.Models.Supplier
{
    public class SupplierMdl
    {
        public int Id { get; set; }
      
        public string Name { get; set; }
        
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
     
        public string State { get; set; }
        public string District { get; set; }
        public int Country { get; set; }
        public string PIN { get; set; }

        public string Pan { get; set; }
        public string Tin { get; set; }
        public string Tan { get; set; }
        public int projectType { get; set; }

        public string countryStr { get; set; }
    }
}