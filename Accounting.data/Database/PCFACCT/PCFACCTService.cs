using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.data.Database.PCFACCT
{
    public class PCFACCTService : IPCFACCTService
    {
        public static SqlConnection _con;
        public void CloseConnection()
        {
            if (_con != null || _con.State == ConnectionState.Open)
            {
                _con.Close();
            }
        }

        public SqlDataReader getIPCFFACTdata(string command)
        {
            using (SqlCommand _cmd = new SqlCommand(command, _con))
            {
                
                SqlDataReader reader = _cmd.ExecuteReader();
                
                return reader;

            }
        }

        public void openconnection()
        {
            if (_con == null)
            {

                _con = new SqlConnection(ConfigurationManager.ConnectionStrings["PCFACCT"].ConnectionString);
                // _con = new SqlConnection("Data Source=USER1-PC;Initial Catalog=PCFACCT;Integrated Security=False;User Id=sa;Password=IcsR@123#;MultipleActiveResultSets=True");
                try
                {
                    _con.Open();
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            if (_con.State == ConnectionState.Closed)
            {
                _con.Open();
            }
        }
    }
}
