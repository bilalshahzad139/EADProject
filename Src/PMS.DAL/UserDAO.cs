using PMS.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.DAL
{
    public static class UserDAO
    {
        public static int Save(UserDTO dto)
        {
            var sqlQuery = "";
            sqlQuery = dto.UserID > 0 ? $"Update dbo.Users Set Name='{dto.Name}', PictureName='{dto.PictureName}' Where UserID={dto.UserID}" : $"INSERT INTO dbo.Users(Name, Login,Password, PictureName, IsAdmin,IsActive) VALUES('{dto.Name}','{dto.Login}','{dto.Password}','{dto.PictureName}',{0},{1})";

            using (var helper = new DBHelper())
            {
                return helper.ExecuteQuery(sqlQuery);
            }
        }

        public static int UpdatePassword(UserDTO dto)
        {
            var sqlQuery = "";
            sqlQuery = $"Update dbo.Users Set Password='{dto.Password}' Where UserID={dto.UserID}";


            using (var helper = new DBHelper())
            {
                return helper.ExecuteQuery(sqlQuery);
            }
        }

        public static UserDTO ValidateUser(string pLogin, string pPassword)
        {
            var query = $"Select * from dbo.Users Where Login='{pLogin}' and Password='{pPassword}'";

            using (var helper = new DBHelper())
            {
                var reader = helper.ExecuteReader(query);

                UserDTO dto = null;

                if (reader.Read()) dto = FillDTO(reader);

                return dto;
            }
        }

        public static UserDTO GetUserById(int pid)
        {

            var query = $"Select * from dbo.Users Where UserId={pid}";

            using (var helper = new DBHelper())
            {
                var reader = helper.ExecuteReader(query);

                UserDTO dto = null;

                if (reader.Read()) dto = FillDTO(reader);

                return dto;
            }
        }

        public static List<UserDTO> GetAllUsers()
        {
            using (var helper = new DBHelper())
            {
                var query = "Select * from dbo.Users Where IsActive = 1;";
                var reader = helper.ExecuteReader(query);
                var list = new List<UserDTO>();

                while (reader.Read())
                {
                    var dto = FillDTO(reader);
                    if (dto != null) list.Add(dto);
                }

                return list;
            }
        }

        public static int DeleteUser(int pid)
        {
            var sqlQuery = $"Update dbo.Users Set IsActive=0 Where UserID={pid}";

            using (var helper = new DBHelper())
            {
                return helper.ExecuteQuery(sqlQuery);
            }
        }

        private static UserDTO FillDTO(SqlDataReader reader)
        {
            var dto = new UserDTO();
            dto.UserID = reader.GetInt32(0);
            dto.Name = reader.GetString(1);
            dto.Login = reader.GetString(2);
            dto.Password = reader.GetString(3);
            dto.PictureName = reader.GetString(4);
            dto.IsAdmin = reader.GetBoolean(5);
            dto.IsActive = reader.GetBoolean(6);

            return dto;
        }
    }
}
