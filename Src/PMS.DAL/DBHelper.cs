using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.DAL
{
    internal class DBHelper : IDisposable
    {
        private readonly string _connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
        
        private readonly SqlConnection _conn = null;

        public DBHelper()
        {
            _conn = new SqlConnection(_connStr);
            _conn.Open();
        }

        public int ExecuteQuery(string sqlQuery)
        {
            var command = new SqlCommand(sqlQuery, _conn);
            var count = command.ExecuteNonQuery();
            return count;
        }

        public object ExecuteScalar(string sqlQuery)
        {
            var command = new SqlCommand(sqlQuery, _conn);
            return command.ExecuteScalar();
        }

        public SqlDataReader ExecuteReader(string sqlQuery)
        {
            var command = new SqlCommand(sqlQuery, _conn);
            return command.ExecuteReader();
        }

        public List<string> ExecuteStoredProcedure(string procedureName, string term)
        {
            var items = new List<string>();
            var command = new SqlCommand(procedureName, _conn) {CommandType = CommandType.StoredProcedure};
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@term",
                Value = term
            });
            var reader = command.ExecuteReader();
            
            while (reader.Read())
            {
                items.Add(reader.GetString(reader.GetOrdinal("Name")));
            }
            return items;
        }

        public void Dispose()
        {
            if (_conn != null && _conn.State == System.Data.ConnectionState.Open)
                _conn.Close();
        }
    }
}
