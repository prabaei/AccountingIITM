using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Accounting.Models.Login
{
    public class LoginMdl
    {
        public string username { get; set; }
        public string password { get; set; }
        public bool rememberme { get; set; }
    }
}