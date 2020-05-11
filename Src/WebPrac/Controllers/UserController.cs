using PMS.BAL;
using PMS.Entities;
using System;
using System.Collections.Generic;
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
							var ext = System.IO.Path.GetExtension(file.FileName);

							//Generate a unique name using Guid
							uniqueName = Guid.NewGuid().ToString() + ext;

							//Get physical path of our folder where we want to save images
							var rootPath = Server.MapPath("~/UploadedFiles/ProfilePics");

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
							urlToRedirect = Url.Content("~/User/Login")
						};
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
		public ActionResult Logout()
		{
			SessionManager.ClearSession();
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