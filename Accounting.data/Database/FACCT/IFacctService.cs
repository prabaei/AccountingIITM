using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.data.Database.FACCT
{
    public interface IFacctService
    {
        SqlDataReader FetchfromFacc(string command);
        int checkTableExistence(string table);
        void openconnection();
    }
}
