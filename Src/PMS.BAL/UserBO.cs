﻿using PMS.DAL;
using PMS.Entities;
using System.Collections.Generic;

namespace PMS.BAL
{
    public static class UserBO
    {
        public static int Save(UserDTO dto)
        {
            return UserDAO.Save(dto);
        }
        public static int Update(UserDTO dto, string previosLogin)
        {
            return UserDAO.Update(dto, previosLogin);
        }

        public static bool isUserAlreadyExist(string pLogin)
        {
            return UserDAO.isUserAlreadyExist(pLogin);
        }
        public static bool isAnotherUserExistExceptActivUser(string pLogin, int UserID)
        {
            return UserDAO.isAnotherUserExistExceptActivUser(pLogin, UserID);
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

        public static string GetSaltForLogin(string login)
        {
            return UserDAO.GetSaltForLogin(login);
        }
        public static List<DistributorDTO> GetDistributors()
        {
            return UserDAO.GetDistributors();
        }
        public static int InsertNewDistributor(DistributorDTO dto)
        {
            return UserDAO.InsertNewDistributor(dto);
        }
    }
}