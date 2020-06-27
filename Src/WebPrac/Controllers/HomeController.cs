using PMS.BAL;
using PMS.Entities;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using WebPrac.Security;

namespace WebPrac.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Admin()
        {
            if (SessionManager.IsValidUser)
            {

                if (SessionManager.User.IsAdmin)
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("NormalUser");
                }
            }
            else
            {
                return Redirect("~/User/Login");
            }
        }
        public ActionResult NormalUser()
        {
            if (SessionManager.IsValidUser)
            {

                if (SessionManager.User.IsAdmin)
                {
                    return RedirectToAction("Admin");
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return Redirect("~/User/Login");
            }
        }
        [HttpGet]
        public JsonResult IsAdmin()
        {
            object data;
            bool a = false;
            if (SessionManager.User.IsAdmin)
            {
                a = true;
            }
            data = new
            {
                admin = a
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult InsertDist(String country, String address, String phone, String website)
        {
            var exc = false;
            Object data;
            var url = Url.Content("~/Home/Distributors");
            DistributorDTO dto = new DistributorDTO();
            dto.Dcountry = country;
            dto.Daddress = address;
            dto.Dphone = phone;
            dto.Dwebsite = website;
            int id = UserBO.InsertNewDistributor(dto);
            if (id == -1)
            {
                exc = true;
            }
            data = new
            {
                urlToRedirect = url,
                exception = exc
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult InsertDistributors()
        {
            if (SessionManager.IsValidUser)
            {

                if (SessionManager.User.IsAdmin)
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Distributors");
                }
            }
            else
            {
                return Redirect("~/User/Login");
            }
        }
        public ActionResult Distributors()
        {
            if (SessionManager.IsValidUser)
            {
                return View();
            }
            else
            {
                return Redirect("~/User/Login");
            }
        }
        [HttpPost]
        public JsonResult displayDistributors()
        {
            List<DistributorDTO> data = UserBO.GetDistributors();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}