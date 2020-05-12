using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.DAL
{
    internal class DBHelper : IDisposable
    {
        String _connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
        //String _connStr="Server=localhost\\SQLEXPRESS;Database=EADProj;User Id=sa;Password=1234;";
        SqlConnection _conn = null;
        public DBHelper()
        {
            _conn = new SqlConnection(_connStr);
            _conn.Open();
        }

        public int ExecuteQuery(String sqlQuery)
        {
            SqlCommand command = new SqlCommand(sqlQuery, _conn);
            var count = command.ExecuteNonQuery();
            return count;
        }
        public Object ExecuteScalar(String sqlQuery)
        {
            SqlCommand command = new SqlCommand(sqlQuery, _conn);
            return command.ExecuteScalar();
        }

        public SqlDataReader ExecuteReader(String sqlQuery)
        {
            SqlCommand command = new SqlCommand(sqlQuery, _conn);
            return command.ExecuteReader();
        }

        public void Dispose()
        {
            if (_conn != null && _conn.State == System.Data.ConnectionState.Open)
                _conn.Close();
        }
    }
}
