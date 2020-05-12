using PMS.DAL;
using PMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.BAL
{
    public static class UserBO
    {
        public static int Save(UserDTO dto)
        {
            return DAL.UserDAO.Save(dto);
        }
        public static Boolean isUserAlreadyExist(String pLogin)
        {
            return UserDAO.isUserAlreadyExist(pLogin);
        }

        public static int UpdatePassword(UserDTO dto)
        {
            return DAL.UserDAO.UpdatePassword(dto);
        }

        public static UserDTO ValidateUser(string pLogin, string pPassword)
        {
            return DAL.UserDAO.ValidateUser(pLogin, pPassword);
        }
        public static UserDTO GetUserById(int pid)
        {
            return DAL.UserDAO.GetUserById(pid);
        }
        public static List<UserDTO> GetAllUsers()
        {
            return DAL.UserDAO.GetAllUsers();
        }

        public static int DeleteUser(int pid)
        {
            return DAL.UserDAO.DeleteUser(pid);
        }

    }
}
