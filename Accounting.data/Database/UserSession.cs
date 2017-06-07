using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.data.Database
{
    [Table("UserSess")]
    public class UserSession
    {
        [Key]
        [StringLength(80)]
        public string userEmail { get; set; }

        public string sessionId { get; set; }

        
    }
}
