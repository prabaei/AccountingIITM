using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.data.Database
{
    public class LoginTbl
    {
        [Key]
        public string StaffEmail { get; set; }
        public byte[] UserPassword { get; set; }
        public string password { get; set; }
        public int Id { get; set; }
    }
}
