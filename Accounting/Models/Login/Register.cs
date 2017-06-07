using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Accounting.Models.Login
{
    public class Register
    {
        public string Name { get; set; }
        public int Gender { get; set; }
        public string Email { get; set; }
        public string password { get; set; }
        public int UsrRole { get; set; }
    }
}