using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.data.Database.PCFACCT
{
    public interface IPCFACCTService
    {

        void openconnection();
        void CloseConnection();

        SqlDataReader getIPCFFACTdata(string command);
    }
}
