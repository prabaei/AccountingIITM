using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Accounting.data.Database
{
    [Table("AccTransaction")]
    public class Transaction
    {

        [Key,Column(Order =1)]
        
        [StringLength(23)]
        public string TransNO { get; set; }
        [StringLength(4)]
        public string INSTID { get; set; }
        [StringLength(25)]
        public string ProjectNo { get; set; }
        public int? voucherType { get; set; }
        [Index("XI_VoucherNo_AccTransaction")]
        [StringLength(10)]
        public string voucherNo { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? bankDate { get; set; }
        public decimal? currentBal { get; set; }
        public decimal? AvailableBal { get; set; }
        [StringLength(15)]
        public string ChequeNO { get; set; }
        [StringLength(250)]
        public string narration { get; set; }
        [StringLength(250)]
        public string Remarks { get; set; }
        [Index("XI_TransCreated_AccTransaction")]
        public DateTime TransCreated { get; set; }
        public DateTime? TransUpdated { get; set; }
        [StringLength(10)]
        public string CommitmentNO { get; set; }
        [Index("XI_TransCommited_AccTransaction")]
        public bool TransCommited { get; set; }
        [StringLength(80)]
        public string UserEmail { get; set; }
        [StringLength(12)]
        public string bnkdate { get; set; }
        [Key,Column(Order =2)]
        [StringLength(14)]
        public string BankAccountNO { get; set; }
        [Index("XI_ProjectType_AccTransaction")]
        public int? ProjectType { get; set; }
        [StringLength(20)]
        public string projtype { get; set; }
        [StringLength(20)]
        public string voucherTypeStr { get; set; }

        [StringLength(100)]
        public string Head { get; set; }

        [StringLength(250)]
        public string Description { get; set; }
        public bool deleted { get; set; }
        public string filterStr
        {
            get
            {
                return filterstring;
            }
            set { filterstring = value; }
        }

        string filterstring = "BANK IMPREST RECOUPM";

        bool pvtengageFilterStr = true;
        public bool engageFilterStr { get { return pvtengageFilterStr; } set { pvtengageFilterStr = value; } }

        public decimal? DEPOSITS { get; set; }

        public decimal? WITHDRAWALS { get; set; }

        public string CoordinatorName { get; set; }

        public bool imported { get; set; }

        public List<SelectListItem> voucherTypeList { get; set; }


        public int? orderId { get; set; }
        
        public bool brsDone { get; set; }

        public IEnumerable<recentTrans> recentHistory { get; set; }
        public bool CNS { get; set; }
        [StringLength(33)]
        public string Recoupid { get; set; }

        public bool RecupDone { get; set; }
        [StringLength(200)]
        public string BankPart { get; set; }

        public int supplier { get; set; }

        public string supplierstr { get; set; }

        public DateTime? ChequeDt { get; set; }

        public string Cheqdt { get; set; }

        public bool MailSent { get; set; }

        public bool MailDelivered { get; set; }

        public bool migrated { get; set; }
    }

    public class recentTrans
    {
        public DateTime? Date { get; set; }
        public decimal? amount { get; set; }

        public string VoucherType { get; set; }

        

    }
}
