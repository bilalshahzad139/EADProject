using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
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

        public static int sendVerificationCode(string email)
        {
            

            Random rnd = new Random();
            int passwordResetCode = rnd.Next(1000, 10000);  // creates a number between 1000 and 10000
            String userEmail = email;
            if (userEmail != "")
            {
                try
                {
                    String fromdisplayEmail = "EAD.SEMorning@gmail.com";
                    String fromPassword = "SEMorning2017";
                    String fromDisplayName = "EAD Shopping Site";
                    MailAddress fromAdress = new MailAddress(fromdisplayEmail, fromDisplayName);
                    MailAddress toAddress = new MailAddress(userEmail);
                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient
                    {

                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(fromAdress.Address, fromPassword)


                    };
                    using (var message = new MailMessage(fromAdress, toAddress)
                    {
                        Subject = "User Management Reset Password",
                        Body = "Your Verification Code is:"+passwordResetCode+"\n Please Do Not Reply to this Email."

                    })
                    {
                        smtp.Send(message);
                    }
                    return UserDAO.storePasswordRecoveryCode(passwordResetCode, userEmail);
                    
                }
                catch (Exception ex)
                {
                    return 0;//error sending email..
                }
            }
            else
                return 0;//Email not Found.
        }
        public static bool isResetPasswordCodeVerified(string code)
        {
            return UserDAO.isResetPasswordCodeVerified(code);
        }

    }
}