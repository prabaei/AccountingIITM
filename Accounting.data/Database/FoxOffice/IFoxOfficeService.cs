using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.data.Database.FoxOffice
{
    public interface IfoxOfficeService
    {
        SqlDataAdapter getPmasterdetail(string APRLNO);
        void openconnection();
        void CloseConnection();

        SqlDataReader getmasterdetailbycmd(string command);
    }
}
