using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Web;
using PMS.BAL;
using PMS.Entities;

namespace WebPrac.Security
{
    public static class EmailVerifier
    {
        public static void SendEmail(UserDTO user)
        {
            try
            {
                var securityCode = EmailVerificationCode();
                var mail = new MailMessage();
                var SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("EAD.SEMorning@gmail.com");
                mail.To.Add(user.Login);
                mail.Subject = "Email Verification";
                mail.IsBodyHtml = true;
                mail.Body = $"<h1 class='text-center' style='color:blue'>Email Verification</h1>" +
                            $"<p>Please click on this link to verify your email address</p>" +
                            $"<a class='text-success' href='http://localhost:59773/User/VerifyEmail?" +
                            $"email={user.Login}&code={securityCode}'>Click to verify email</a>";

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("EAD.SEMorning@gmail.com", "SEMorning2017");
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);
                UserBO.CodeInsertion(user, securityCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private static string EmailVerificationCode()
        {
            var n = "";
            int randomValue;
            using (var crypto = new RNGCryptoServiceProvider())
            {
                var val = new byte[6];
                crypto.GetBytes(val);
                randomValue = BitConverter.ToInt32(val, 0);
            }
            n += randomValue.ToString().Substring(1, 1)[0];
            n += randomValue.ToString().Substring(2, 1)[0];
            n += randomValue.ToString().Substring(3, 1)[0];
            n = n + "-";
            n += randomValue.ToString().Substring(4, 1)[0];
            n += randomValue.ToString().Substring(5, 1)[0];
            n += randomValue.ToString().Substring(6, 1)[0];
            return n;
        }
    }

}