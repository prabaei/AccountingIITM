using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Accounting.data.Database.FoxOffice
{
    public  class FoxOfficeService:IfoxOfficeService
    {
        public static SqlConnection _con;
       public   void  openconnection()
        {
            
          if(_con == null ) 
            {

                 _con = new SqlConnection(ConfigurationManager.ConnectionStrings["Foxoffice"].ConnectionString);
               // _con = new SqlConnection("Data Source=USER1-PC;Initial Catalog=PCFACCT;Integrated Security=False;User Id=sa;Password=IcsR@123#;MultipleActiveResultSets=True");
                try
                {
                    _con.Open();
                }catch(Exception ex)
                {
                    throw ex;
                }
                
            }
          if(_con.State==ConnectionState.Closed)
            {
                _con.Open();
            }
            
        }
        public  SqlDataAdapter getPmasterdetail(string APRLNO)
        {
            using (SqlCommand _cmd = new SqlCommand("GETPMASTERDETAIL", _con))
            {
                _cmd.CommandType = CommandType.StoredProcedure;

                _cmd.Parameters.Add(new SqlParameter("@APRLNO", SqlDbType.NVarChar));
                _cmd.Parameters["@APRLNO"].Value = APRLNO;

                SqlDataAdapter _dap = new SqlDataAdapter(_cmd);
                return _dap;
                
            }
        }

        public void CloseConnection()
        {
            if (_con != null || _con.State == ConnectionState.Open)
            {
                _con.Close();
            }
        }

        public SqlDataReader getmasterdetailbycmd(string command)
        {
            using (SqlCommand _cmd = new SqlCommand(command, _con))
            {
                //_cmd.CommandType = CommandType.StoredProcedure;

                //_cmd.Parameters.Add(new SqlParameter("@APRLNO", SqlDbType.NVarChar));
                //_cmd.Parameters["@APRLNO"].Value = APRLNO;
                //_cmd.ExecuteNonQuery();
                SqlDataReader reader = _cmd.ExecuteReader();
                //SqlDataAdapter _dap = new SqlDataAdapter(_cmd);
                return reader;

            }
        }
    }
}
