using PMS.BAL;
using PMS.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
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
		public ActionResult Login(String login, String password)
		{
			var obj = PMS.BAL.UserBO.ValidateUser(login, password);
			if (obj != null)
			{
				Session["user"] = obj;
				if (obj.IsAdmin == true)
					return Redirect("~/Home/Admin");
				else
					return Redirect("~/Home/NormalUser");
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
		// I am goin to hit it by AJAX
		[HttpPost]
		[ActionName("Signup")]
		public ActionResult Signup(UserDTO userDto)
		{
			Object result = null;
			if (userDto.Login.IsEmpty() || userDto.Name.IsEmpty() || userDto.Password.IsEmpty() || userDto.PictureName.IsEmpty())
			{
				ViewBag.ErrMsg = "Empty Fields!";
				return View("Signup");
			}
			try
			{
				Boolean isUserAlreadyExist = UserBO.isUserAlreadyExist(userDto.Login);
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
							var ext = System.IO.Path.GetExtension(file.FileName);

							//Generate a unique name using Guid
							uniqueName = Guid.NewGuid().ToString() + ext;

							//Get physical path of our folder where we want to save images
							var rootPath = Server.MapPath("~/ProfilePictures");

							var fileSavePath = System.IO.Path.Combine(rootPath, uniqueName);

							// Save the uploaded file to "UploadedFiles" folder
							file.SaveAs(fileSavePath);

							userDto.PictureName = uniqueName;
						}
					}
					
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
					result = new
					{
						isUserExist = isUserAlreadyExist,
						urlToRedirect = ""
					};
				}
			}
			catch (Exception ex)
			{
				result = new
				{
					isUserExist = false,
					urlToRedirect = ""
				};
			}
			return Json(result, JsonRequestBehavior.AllowGet);
		}

        [HttpGet]
        public ActionResult VerifyEmail(string email , string code)
        {
            if (string.IsNullOrEmpty(email)||string.IsNullOrEmpty(code))
            {
                return View($"VerifyEmail");
            }
            var user = new UserDTO()
            {
                Login =  email,Password = "",IsActive = false
            };
            if (UserBO.EmailVerification(user,code))
            {
                ViewBag.EmailVerified = true;
                return Redirect("~/User/Login");
            }
            ViewData["server-error"] = "Something has gone wrong";
            return View($"VerifyEmail");
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
			
			feedbackBO.saveFeedBack(sos);
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
	}
}