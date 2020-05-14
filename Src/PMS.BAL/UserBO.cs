using System.Collections.Generic;
using PMS.DAL;
using PMS.Entities;

namespace PMS.BAL
{
    public static class UserBO
    {
        public static int Save(UserDTO dto)
        {
            return UserDAO.Save(dto);
        }

        public static bool isUserAlreadyExist(string pLogin)
        {
            return UserDAO.isUserAlreadyExist(pLogin);
        }

        public static int UpdatePassword(UserDTO dto)
        {
            return UserDAO.UpdatePassword(dto);
        }

        public static UserDTO ValidateUser(string pLogin, string pPassword)
        {
            return UserDAO.ValidateUser(pLogin, pPassword);
        }

        public static UserDTO GetUserById(int pid)
        {
            return UserDAO.GetUserById(pid);
        }

        public static List<UserDTO> GetAllUsers()
        {
            return UserDAO.GetAllUsers();
        }

        public static int DeleteUser(int pid)
        {
            return UserDAO.DeleteUser(pid);
        }

        public static bool CodeInsertion(UserDTO user, string code)
        {
            return UserDAO.CodeInsertion(user, code);
        }

        public static bool EmailVerification(UserDTO user, string code)
        {
            return UserDAO.VerifyEmail(user, code);
        }
    }
}