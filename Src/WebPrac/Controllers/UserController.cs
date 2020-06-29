using Microsoft.Ajax.Utilities;
using PMS.BAL;
using PMS.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Configuration;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using WebPrac.Security;

namespace WebPrac.Controllers
{
    public class UserController : Controller
    {

        [HttpGet]
        public ActionResult ShowUsers()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetAllUsers()
        {
            List<UserDTO> l1 = new List<UserDTO>();
            l1 = PMS.BAL.UserBO.GetAllUsers();
            return Json(l1, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetUserByID(int ID)
        {
            UserDTO d1 = new UserDTO();
            d1 = PMS.BAL.UserBO.GetUserById(ID);
            return Json(d1, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Update(UserDTO dto, string previosLogin)
        {
            int recAff = PMS.BAL.UserBO.Update(dto, previosLogin);
            return Json(recAff, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string login, string password)
        {
            var user = new UserDTO { Login = login, Password = password, PswSalt = UserBO.GetSaltForLogin(login) };
            if (user.Login != "admin@gmail.com")
            {
                if (!string.IsNullOrEmpty(user.PswSalt))
                {
                    UserPswHashing.GenerateHash(user);
                    var obj = UserBO.ValidateUser(user.Login, user.Password);

                    if (obj != null)
                    {
                        Session["user"] = obj;

                        return Redirect(obj.IsAdmin ? "~/Home/Admin" : "~/Home/NormalUser");
                    }
                }
            }
            else
            {
                var obj = UserBO.ValidateUser(user.Login, user.Password);
                if (obj != null)
                {
                    Session["user"] = obj;
                    return Redirect(obj.IsAdmin ? "~/Home/Admin" : "~/Home/NormalUser");
                }
            }
            ViewBag.MSG = "Invalid Login/Password";
            ViewBag.Login = login;
            return View();
        }

        [HttpGet]
        public ActionResult Signup()
        {
            ViewBag.Title = "Sign up";
            return View();
        }

        // I am going to hit it by AJAX
        [HttpPost]
        [ActionName("Signup")]
        public ActionResult Signup(UserDTO userDto)
        {
            object result = null;
            if (!userDto.IsValid())
            {
                ViewBag.ErrMsg = "Empty Fields!";
                return View("Signup");
            }

            try
            {
                //Server side Email validation
                string expression = @"^[a-z][a-z|0-9|]*([_][a-z|0-9]+)*([.][a-z|" + @"0-9]+([_][a-z|0-9]+)*)?@[a-z][a-z|0-9|]*\.([a-z]" + @"[a-z|0-9]*(\.[a-z][a-z|0-9]*)?)$";
                Match match = Regex.Match(userDto.Login, expression, RegexOptions.IgnoreCase);
                if (!match.Success)
                {
                    ViewBag.ErrMsg = "You have entered Invalid Email Address!";
                    return View("Signup");
                }

                var isUserAlreadyExist = UserBO.IsUserAlreadyExist(userDto.Login);
                if (!isUserAlreadyExist)
                {
                    //Picture handling
                    var uniqueName = "";
                    if (Request.Files["myProfilePic"] != null)
                    {
                        var file = Request.Files["myProfilePic"];
                        if (file.FileName != "")
                        {
                            Directory.CreateDirectory(Server.MapPath("~/ProfilePictures"));
                            var ext = Path.GetExtension(file.FileName);

                            //Generate a unique name using Guid
                            uniqueName = Guid.NewGuid() + ext;

                            //Get physical path of our folder where we want to save images
                            var rootPath = Server.MapPath("~/ProfilePictures");
                            var fileSavePath = Path.Combine(rootPath, uniqueName);

                            // Save the uploaded file to "UploadedFiles" folder
                            file.SaveAs(fileSavePath);
                            userDto.PictureName = uniqueName;
                        }
                    }

                    userDto.IsActive = false;
                    userDto.PswSalt = UserPswHashing.CreateSalt();
                    UserPswHashing.GenerateHash(userDto);
                    var res = UserBO.Save(userDto);
                    if (res == 1)
                    {
                        result = new
                        {
                            isUserExist = isUserAlreadyExist,
                            urlToRedirect = Url.Content("~/User/VerifyEmail")
                        };
                        EmailVerifier.SendEmail(userDto);
                    }
                }
                else
                {
                    result = new { isUserExist = isUserAlreadyExist, urlToRedirect = "" };
                }
            }
            catch (Exception ex)
            {
                result = new { isUserExist = false, urlToRedirect = "" };
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult VerifyEmail(string email, string code)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code)) return View("VerifyEmail");
            var user = new UserDTO { Login = email, Password = "", IsActive = false };
            if (UserBO.EmailVerification(user, code))
            {
                ViewBag.EmailVerified = true;
                return Redirect("~/User/Login");
            }

            ViewData["server-error"] = "Something has gone wrong";
            return View("VerifyEmail");
        }


		[HttpGet]

		//feedback view will be open without an active user so
		//So i Changed session manager possion
		public ActionResult Logout()
		{
			UserDTO u = (UserDTO)Session["user"];
			if (u.IsAdmin == true)
			{
				//if user is admin, session will be cleared and feedback page will not open
				SessionManager.ClearSession();
				return RedirectToAction("Login");
			}
			else
				return RedirectToAction("Feedback");
		}

		[HttpPost]
		public ActionResult Feedback(feedbackDTO sos)
		{
			
			feedbackBO.saveFeedBack(sos.name,sos.message);
			SessionManager.ClearSession();
			return RedirectToAction("Login");
		}

		[HttpGet]
		public ActionResult Feedback()
		{
			//if user is regularUser session contain something, hence feedback view opened
			if(Session["user"]!=null)
			{
				return View("Feedback");
			}
			return RedirectToAction("Login");
		}
		[HttpGet]
		public ActionResult ForgotPassword()
		{
			return View();
		}
		
		[HttpPost]
		public JsonResult SendVerificationCode(string email)
		{
			
			if(UserBO.IsUserAlreadyExist(email))
			{

				int code = UserBO.SendVerificationCode(email);
				if(code!=0)//Code has been sent.
				{
					
					var h = new
					{
						statusbit = 1,
						msg = "Code successfully sent to the email",
						data = email
					};
					return Json(h, JsonRequestBehavior.AllowGet);
				}
				 var b = new
				{
					statusbit = -1,
					msg = "Error Sending Code",
					data = code
				};
				return Json(b, JsonRequestBehavior.AllowGet);
			}

			 var k = new
			{
				statusbit = 0,
				msg = "User Doesnt Exist",
			};
			return Json(k, JsonRequestBehavior.AllowGet);
		}


		[HttpPost]
		public JsonResult VerifyCode(string verificationCode)
		{
			{

				bool isVerified = UserBO.IsResetPasswordCodeVerified(verificationCode);
				if (isVerified)//Code has been sent.
				{

					var h = new
					{
						statusbit = 1,
						msg = "Code Verified Successfully.",
						data = verificationCode
					};
					return Json(h, JsonRequestBehavior.AllowGet);
				}
				var b = new
				{
					statusbit = -1,
					msg = "Code Not Verified",
					data = verificationCode
				};
				return Json(b, JsonRequestBehavior.AllowGet);
			}
		}
        [HttpGet]
        public ActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        public JsonResult ResetPassword(string email,string newPassword)
        {
            int isChanged = UserBO.ResetPassword(email, newPassword);
            if (isChanged == 1)
            {
                var h = new
                {
                    statusbit = 1,
                    msg = "Password Changed Succeccfull",
                    data = email
                };
                ViewBag.MSG = "Password Successfully changed for " + email;
                return Json(h, JsonRequestBehavior.AllowGet);
            }
            var b = new
            {
                statusbit = -1,
                msg = "Password NOT CHANGED.",
                data = email
            };
            ViewBag.MSG = "Error in changing password for " + email;
            return Json(b, JsonRequestBehavior.AllowGet);
        }
		//[HttpGet]
		//public ActionResult Login2()
		//{
		//    return View();
		//}

		//[HttpPost]
		//public JsonResult ValidateUser(String login, String password)
		//{

		//    Object data = null;

		//    try
		//    {
		//        var url = "";
		//        var flag = false;

		//        var obj = PMS.BAL.UserBO.ValidateUser(login, password);
		//        if (obj != null)
		//        {
		//            flag = true;
		//            SessionManager.User = obj;

		//            if (obj.IsAdmin == true)
		//                url = Url.Content("~/Home/Admin");
		//            else
		//                url = Url.Content("~/Home/NormalUser");
		//        }

		//        data = new
		//        {
		//            valid = flag,
		//            urlToRedirect = url
		//        };
		//    }
		//    catch (Exception)
		//    {
		//        data = new
		//        {
		//            valid = false,
		//            urlToRedirect = ""
		//        };
		//    }

		//    return Json(data, JsonRequestBehavior.AllowGet);
		//}
	

        [HttpGet]

        //feedback view will be open without an active user so
        //So i Changed session manager possion
        

        public ActionResult UpdateProfile()
        {
            if (Session["user"] != null)
            {
                var activeUser = (UserDTO)Session["user"];
                ViewBag.Login = activeUser.Login;
                ViewBag.Name = activeUser.Name;
                ViewBag.PictureName = activeUser.PictureName;
                return View("UpdateProfile");
            }

            return RedirectToAction($"Login");
        }






        [HttpPost]
        public JsonResult UpdateProfile(UserDTO userDTO)
        {
            if (string.IsNullOrEmpty(userDTO.Login) || string.IsNullOrEmpty(userDTO.Name) ||
                string.IsNullOrEmpty(userDTO.Password))
            {
                var data = new { success = 2, result = "Please Fill in All the Fields..." };
                return Json(data, JsonRequestBehavior.AllowGet);
            }

            //Server side Email validation
            string expression = @"^[a-z][a-z|0-9|]*([_][a-z|0-9]+)*([.][a-z|" + @"0-9]+([_][a-z|0-9]+)*)?@[a-z][a-z|0-9|]*\.([a-z]" + @"[a-z|0-9]*(\.[a-z][a-z|0-9]*)?)$";
            Match match = Regex.Match(userDTO.Login, expression, RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                var data = new { success = 2, result = "You have entered an invalid email address...Please Enter valid email...!!!" };
                return Json(data, JsonRequestBehavior.AllowGet);
            }

            var activeUser = (UserDTO)Session["user"];
            if (!UserBO.isAnotherUserExistExceptActivUser(userDTO.Login, activeUser.UserID))
            {
                userDTO.UserID = activeUser.UserID;

                //Picture handling

                var uniqueName ="";

                if (Request.Files["myProfilePic"] != null)
                {
                    var file = Request.Files["myProfilePic"];
                    if (file.FileName != "")
                    {
                        Directory.CreateDirectory(Server.MapPath("~/ProfilePictures"));
                        var ext = Path.GetExtension(file.FileName);

                        //Generate a unique name using Guid
                        uniqueName = Guid.NewGuid() + ext;

                        //Get physical path of our folder where we want to save images
                        var rootPath = Server.MapPath("~/ProfilePictures");
                        var fileSavePath = Path.Combine(rootPath, uniqueName);

                        // Save the uploaded file to "UploadedFiles" folder
                        file.SaveAs(fileSavePath);
                        userDTO.PictureName = uniqueName;
                    }
                }
                userDTO.PswSalt = UserPswHashing.CreateSalt();
                UserPswHashing.GenerateHash(userDTO);

                var updateResult = UserBO.Update(userDTO,activeUser.Login);

                if (updateResult > 0)
                {
                    activeUser.Name = userDTO.Name;
                    activeUser.Password = userDTO.Password;
                    activeUser.Login = userDTO.Login;
                    activeUser.PictureName = userDTO.PictureName;
                    Session["user"] = activeUser;
                    EmailVerifier.SendEmail(userDTO);
                    var data = new { success = 1, result = "Updated Successfully..." };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var data = new { success = 0, result = "Some Error Occcured while Updating..." };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var data = new { success = 2, result = "User ALready Exist...Please Try again with another 'Login'" };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }




        [HttpPost]
        public ActionResult Feedback(string msg)
        {
            var n = (UserDTO)Session["user"];
            feedbackBO.saveFeedBack(msg,n.Name);

            SessionManager.ClearSession();
            return RedirectToAction("Login");
        }

    }
}

