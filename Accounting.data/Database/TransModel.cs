using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Accounting.data.Database
{
    public class TransModel
    {
       
        public string TransNO { get; set; }
        public string INSTID { get; set; }
        public string ProjectNo { get; set; }
        public int? voucherType { get; set; }
 
        public string voucherNo { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? bankDate { get; set; }
        public decimal? currentBal { get; set; }
        public decimal? AvailableBal { get; set; }
        public string ChequeNO { get; set; }
        public string narration { get; set; }
        public string Remarks { get; set; }

        public DateTime TransCreated { get; set; }
        public DateTime? TransUpdated { get; set; }
        public string CommitmentNO { get; set; }

        public bool TransCommited { get; set; }
    
        public string UserEmail { get; set; }
    
        public string bnkdate { get {
                if (bankDate != null)
                {
                    return Convert.ToDateTime(bankDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                else
                {
                    return "";
                }
            } }
      
        public string BankAccountNO { get; set; }

        public int? ProjectType { get; set; }

        public string projtype { get; set; }

        public string voucherTypeStr { get; set; }

        public string head { get; set; }
        public string desc { get; set; }
        
        public bool deleted { get; set; }

        public decimal? DEPOSITS { get; set; }

        public decimal? WITHDRAWALS { get; set; }

        public string CoordinatorName { get; set; }

        public bool imported { get; set; }
        public int? orderId { get; set; }

        public bool brsDone { get; set; }

        public string openingBalance { get; set; }

        public bool CNS { get; set; }

        public string Recoupid { get; set; }

        public bool RecupDone { get; set; }

        public string bankPart { get; set; }  

        public bool credit { get; set; }

        public  bool recoup { get; set; }

        public string Supplier { get; set; }

        public bool Migrated { get; set; }

        public bool brsEntry { get; set; }
    }
}