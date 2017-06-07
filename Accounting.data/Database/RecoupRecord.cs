using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.data.Database
{
    [Table("RecoupRecord")]
    public class RecoupRecord
    {
        [Key]
        [StringLength(33)]
        public string RecoupId { get; set; }
        [Index("XI_AccountNo_RecoupRecord")]
        [StringLength(25)]
        public string AccountNo { get; set; }
        public decimal? Amount { get; set; }
        public DateTime Created { get; set; }

        public string UserID { get; set; }
        public bool Deleted { get; set; }
    }
}
