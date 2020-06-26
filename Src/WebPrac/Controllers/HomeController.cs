using PMS.BAL;
using PMS.Entities;
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