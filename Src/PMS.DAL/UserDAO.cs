﻿using PMS.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace PMS.DAL
{
    public static class UserDAO
    {
        public static int Save(UserDTO dto)
        {
            int count = 0, count2 = 0;
            var sqlQuery = "";
            sqlQuery = dto.UserID > 0
                ? $"Update dbo.Users Set Name='{dto.Name}', PictureName='{dto.PictureName}' Where UserID={dto.UserID}"
                : $"INSERT INTO dbo.Users(Name, Login,Password, PictureName, IsAdmin,IsActive) VALUES('{dto.Name}','{dto.Login}','{dto.Password}','{dto.PictureName}',{0},'{dto.IsActive}');";

            using (var helper = new DBHelper())
            {
                count = helper.ExecuteQuery(sqlQuery);
                sqlQuery = $"INSERT INTO dbo.UserPswSalt (Login, salt) VALUES('{dto.Login}','{dto.PswSalt}')";
                count2 = helper.ExecuteQuery(sqlQuery);
            }

            return count;
        }

        public static bool VerifyEmail(UserDTO user, string code)
        {
            var flag = false;
            var query =
                $"SELECT * FROM dbo.EmailVerifyingCodes WHERE email = '{user.Login}' AND verification_code = '{code}' AND expired != 'true'";
            using (var dbh = new DBHelper())
            {
                var reader = dbh.ExecuteReader(query);
                if (!reader.Read()) return flag;
                flag = true;
                reader.Dispose();
                query = $"UPDATE dbo.Users SET IsActive = 'true' WHERE Login = '{user.Login}'";
                var recAff = dbh.ExecuteQuery(query);
                user.IsActive = recAff == 1;
                query = $"UPDATE dbo.EmailVerifyingCodes SET expired = 'true' WHERE email='{user.Login}'";
                recAff = dbh.ExecuteQuery(query);
            }

            return flag;
        }

        public static bool CodeInsertion(UserDTO user, string code)
        {
            var flag = false;
            using (var dbh = new DBHelper())
            {
                var query =
                    $"SELECT COUNT(*) FROM dbo.EmailVerifyingCodes WHERE email = '{user.Login}' AND expired = 'false'";
                var read = (int)dbh.ExecuteScalar(query);
                query = read == 0
                    ? $"INSERT INTO dbo.EmailVerifyingCodes VALUES('{user.Login}','{code}','false')"
                    : $"UPDATE dbo.EmailVerifyingCodes SET verification_code = '{code}' WHERE email = '{user.Login}'";

                var recAff = dbh.ExecuteQuery(query);
                if (recAff != 1) return flag;

                flag = true;
            }

            return flag;
        }

        public static bool isUserAlreadyExist(string pLogin)
        {
            var mySQLQuery = string.Format(@"SELECT count(*) FROM dbo.Users WHERE login = '{0}'", pLogin);
            using (var dbh = new DBHelper())
            {
                var result = Convert.ToInt32(dbh.ExecuteScalar(mySQLQuery));
                if (result != 0)
                    return true;
                return false;
            }
        }

        public static bool isAnotherUserExistExceptActivUser(string pLogin, int UserID)
        {
            var mySQLQuery = string.Format(@"SELECT count(*) FROM dbo.Users WHERE login = '{0}' and UserID!='{1}'",
                pLogin, UserID);
            using (var dbh = new DBHelper())
            {
                var result = Convert.ToInt32(dbh.ExecuteScalar(mySQLQuery));
                if (result != 0)
                    return true;
                return false;
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

        public static int Update(UserDTO dto)
        {
            var recAff = 0;
            var sqlQuery =
                $"Update dbo.Users Set Password='{dto.Password}' , PictureName='{dto.PictureName}' ,  Name='{dto.Name}', Login='{dto.Login}' Where UserID={dto.UserID}";

            using (var helper = new DBHelper())
            {
                try
                {
                    recAff =  helper.ExecuteQuery(sqlQuery);
                    sqlQuery = $"UPDATE dbo.UserPswSalt SET salt='{dto.PswSalt}' WHERE Login='{dto.Login}'";
                    _ = helper.ExecuteQuery(sqlQuery);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return recAff;
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

        public static string GetSaltForLogin(string login)
        {
            string salt = null;
            var selectQuery = $"SELECT salt FROM dbo.UserPswSalt WHERE Login = '{login}'";
            using (var helper = new DBHelper())
            {
                var reader = helper.ExecuteReader(selectQuery);
                if (reader.Read())
                {
                    salt = reader.GetString(reader.GetOrdinal("salt"));
                }
            }
            return salt;
        }
    }
}