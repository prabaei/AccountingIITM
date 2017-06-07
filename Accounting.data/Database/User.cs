using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.data.Database
{
    [Table("UserTbl")]
    public class User
    {
        [StringLength(150)]
        public string Name { get; set; }
        public int Gender { get; set; }
        [Key]
        [StringLength(80)]
        public string Email { get; set; }
        public byte[] UserPassword { get; set; }
        public string password { get; set; }
        
        public int UsrRole { get; set; }

        public bool NewReg { get; set; }
    }
}
