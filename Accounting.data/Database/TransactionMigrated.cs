using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.data.Database
{
    [Table("Migrated")]
   public class TransactionMigrated
    {
        [Key]
        [Column(Order =1)]
       [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Key]
        [Column(Order = 2)]
        public string AccountNo { get; set; }
        public int Autoid { get; set; }
        public decimal? TallyMasterId { get; set; }
        [StringLength(100)]
        public string voucherId { get; set; }
        [StringLength(100)]
        public string VoucherNumber { get; set; }

        public DateTime? voucherDate { get; set; }
        [StringLength(100)]
        public string voucherType { get; set; }

        [StringLength(100)]
        public string LedgerName { get; set; }
        [StringLength(100)]
        public string BillName { get; set; }

        public bool Credit { get; set; }

        public decimal? Amount { get; set; }

        [StringLength(100)]
        public string Narration { get; set; }
        [StringLength(100)]
        public string InstrumentNO { get; set;}
        [StringLength(100)]
        public string InstrumentType { get; set; }
        [StringLength(100)]
        public string InstrumentBank { get; set; }
        public DateTime? addDate { get; set; }
        [StringLength(100)]
        public string UserName { get; set; }
        [StringLength(100)]
        public string projectNo { get; set; }
        [StringLength(100)]
        public string appledgername { get; set; }
        public string CostCentreName { get; set; }
        public decimal currentBal { get; set; }
    }
}
