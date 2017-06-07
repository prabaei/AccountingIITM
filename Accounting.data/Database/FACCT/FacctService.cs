using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.data.Database.FACCT
{
    public class FacctService : IFacctService
    {
        public static SqlConnection _con;
        public void openconnection()
        {

            if (_con == null)
            {

                _con = new SqlConnection(ConfigurationManager.ConnectionStrings["FACCT"].ConnectionString);
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


        public void CloseConnection()
        {
            if (_con != null || _con.State == ConnectionState.Open)
            {
                _con.Close();
            }
        }


        public SqlDataReader FetchfromFacc(string command)
        {
            
            using (SqlCommand _cmd = new SqlCommand(command, _con))
            {
                try
                {
                    SqlDataReader reader = _cmd.ExecuteReader();
                    return reader;
                }
                catch (Exception ex)
                {
                    throw ex;
                }


            }
        }
        public int checkTableExistence(string tablename)
        {
           
            string command = string.Format("select case when exists((select * from information_schema.tables where table_name = '{0}'))then 1 else 0 end", tablename);
            using (SqlCommand _cmd = new SqlCommand(command, _con))
            {
                try
                {
                    return (int)_cmd.ExecuteScalar();

                }
                catch (Exception ex) { throw ex; }
            }
        }
    }
}
