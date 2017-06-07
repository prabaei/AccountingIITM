using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.data.Database
{
    [Table("ErrorLog")]
    public class ErrorLog
    {
        [Key]
        public int Id { get; set; }
        [StringLength(250)]
        public string Error { get; set; }
        [StringLength(80)]
        public string UserName { get; set; }
        [Index("XI_LoggedAt_ErrorLog")]
        public DateTime LoggedAt { get; set; }
    }
}
