using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.data.Database
{
    [Table("ChequeNotDep")]
    public class ChequeNotDep
    {
        [Key]
        [StringLength(23)]
        [Column(Order =1)]
        public string Tranno { get; set; }
        [Key]
        [Column(Order = 2)]
        [StringLength(20)]
        public string Accno { get; set; }

        
    }

    [Table("ClimNotSubmitted")]
    public class ClimNotSubmitted
    {
        [Key]
        [StringLength(23)]
        [Column(Order = 1)]
        public string Tranno { get; set; }
        [Key]
        [Column(Order = 2)]
        [StringLength(20)]
        public string Accno { get; set; }

        public bool mailsent { get; set; }

        public bool mailDelivered { get; set; }


    }
}
