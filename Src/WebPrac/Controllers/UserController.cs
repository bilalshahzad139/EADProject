using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.WebPages;
using PMS.BAL;
using PMS.Entities;
using WebPrac.Security;

namespace WebPrac.Controllers
{
    public class UserController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string login, string password)
        {
            var user = new UserDTO {Login = login, Password = password, PswSalt = UserBO.GetSaltForLogin(login)};
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

                var isUserAlreadyExist = UserBO.isUserAlreadyExist(userDto.Login);
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

                    userDto.IsActive = true;
                    userDto.PswSalt = UserPswHashing.CreateSalt();
                    UserPswHashing.GenerateHash(userDto);
                    var res = UserBO.Save(userDto);
                    if (res == 1)
                    {
                        result = new
                        {
                            isUserExist = isUserAlreadyExist, urlToRedirect = Url.Content("~/User/VerifyEmail")
                        };
                        EmailVerifier.SendEmail(userDto);
                    }
                }
                else
                {
                    result = new {isUserExist = isUserAlreadyExist, urlToRedirect = ""};
                }
            }
            catch (Exception ex)
            {
                result = new {isUserExist = false, urlToRedirect = ""};
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult VerifyEmail(string email, string code)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code)) return View("VerifyEmail");
            var user = new UserDTO {Login = email, Password = "", IsActive = false};
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
            var u = (UserDTO) Session["user"];
            if (u.IsAdmin)
            {
                //if user is admin, session will be cleared and feedback page will not open
                SessionManager.ClearSession();
                return RedirectToAction("Login");
            }

            return RedirectToAction("Feedback");
        }

        public ActionResult UpdateProfile()
        {
            if (Session["user"] != null)
            {
                var activeUser = (UserDTO) Session["user"];
                ViewBag.Login = activeUser.Login;
                ViewBag.Name = activeUser.Name;
                return View("UpdateProfile");
            }

            return RedirectToAction("Login");
        }

        [HttpPost]
        public JsonResult UpdateProfile(UserDTO userDTO)
        {
            if (string.IsNullOrEmpty(userDTO.Login) || string.IsNullOrEmpty(userDTO.Name) ||
                string.IsNullOrEmpty(userDTO.Password))
            {
                var data = new {success = 2, result = "Please Fill in All the Fields..."};
                return Json(data, JsonRequestBehavior.AllowGet);
            }

            var activeUser = (UserDTO) Session["user"];
            if (!UserBO.isAnotherUserExistExceptActivUser(userDTO.Login, activeUser.UserID))
            {
                userDTO.UserID = activeUser.UserID;
                var updateResult = UserBO.Update(userDTO);
                if (updateResult > 0)
                {
                    activeUser.Name = userDTO.Name;
                    activeUser.Password = userDTO.Password;
                    activeUser.Login = userDTO.Login;
                    Session["user"] = activeUser;
                    var data = new {success = 1, result = "Updated Successfully..."};
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var data = new {success = 0, result = "Some Error Occcured while Updating..."};
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }

            {
                var data = new {success = 2, result = "User ALready Exist...Please Try again with another 'Login'"};
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Feedback(feedbackDTO sos)
        {
            feedbackBO.saveFeedBack(sos);
            SessionManager.ClearSession();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Feedback()
        {
            //if user is regularUser session contain something, hence feedback view opened
            if (Session["user"] != null) return View("Feedback");
            return RedirectToAction("Login");
        }
    }
}