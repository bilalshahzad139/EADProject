using PMS.BAL;
using PMS.Entities;
using System;
using System.Globalization;
using System.Web.Mvc;
using System.Web.WebPages;
using WebPrac.Security;

namespace WebPrac.Controllers
{
    public class Product2Controller : Controller
    {
        public ActionResult New()
        {
            return View();
        }
        public JsonResult GetAllProducts()
        {
            var products = PMS.BAL.ProductBO.GetAllProducts(true);
            var d = new
            {
                data = products
            };
            return Json(d, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddCategory()
        {
            return View();
        }
        [HttpPost]
        public JsonResult AddLikes(int ProductID)
        {
            var activeUser = (UserDTO)Session["user"];
            var result = PMS.BAL.ProductBO.AddLikesAndGetCount(ProductID, activeUser.UserID);
            var d = new
            {
                data = result
            };
            return Json(d, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AddDisLikes(int ProductID)
        {
            var activeUser = (UserDTO)Session["user"];
            var result = PMS.BAL.ProductBO.AddDisLikesAndGetCount(ProductID, activeUser.UserID);
            var d = new
            {
                data = result
            };
            return Json(d, JsonRequestBehavior.AllowGet);
        }
 
        public JsonResult AddCategoryinDatabase(String Cat_name)
        {

            object d;
            if (Cat_name == "")
            {
                d = new
                {
                    valid = false
                };
            }
            else
            {
                var dto = PMS.BAL.ProductCategoryBO.GetCategory(Cat_name);
                if (dto != null)
                {
                    d = new
                    {
                        valid = false
                    };
                }
                else
                {
                    var result = PMS.BAL.ProductCategoryBO.Save(Cat_name);
                    d = new
                    {
                        valid = true
                    };
                }
            }
            return Json(d, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProductsByCategory(int id)
        {
            
            Object products = PMS.BAL.ProductBO.GetProductsByCategory(id, true);
            var d = new
            {
                data = products
            };
            return Json(d, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllCategories()
        {
            var productCategories = PMS.BAL.ProductCategoryBO.GetAllCategories();

            var d = new
            {
                data = productCategories
            };
            return Json(d, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetPriceRangedProducts(int from, int to)
        {

            var products = PMS.BAL.ProductBO.GetPriceRangedProducts(from, to, true);

            var d = new
            {
                data = products
            };
            return Json(d, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProductById(int pid)
        {
            var prod = PMS.BAL.ProductBO.GetProductById(pid);
            var d = new
            {
                data = prod
            };
            return Json(d, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult DeleteProduct(int pid)
        {
            PMS.BAL.ProductBO.DeleteProduct(pid);
            var data = new
            {
                success = true
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Save(ProductDTO dto)
        {
         
            if (dto.Name.IsEmpty() || Convert.ToString(dto.Price, CultureInfo.InvariantCulture).IsEmpty() || Convert.ToString(dto.Quantity, CultureInfo.InvariantCulture).IsEmpty())
            {
                ViewBag.EmptyFiledsMsg = "Empty Fields!";
                return View("New");
            }
            if (dto.PictureName.IsEmpty() && Request.Files["Image"] == null)
            {
                ViewBag.EmptyFiledsMsg = "Click on Choose File to upload Picture of Product!";
                return View("New");
            }
            if (Request.Files["Image"] != null)
            {
               
                var file = Request.Files["Image"];
                if (file.FileName != "")
                {
                    var ext = System.IO.Path.GetExtension(file.FileName);
                    //Generate a unique name using Guid
                    var uniqueName = Guid.NewGuid().ToString() + ext;
                    //Get physical path of our folder where we want to save images
                    var rootPath = Server.MapPath("~/UploadedFiles");
                    var fileSavePath = System.IO.Path.Combine(rootPath, uniqueName);
                    // Save the uploaded file to "UploadedFiles" folder
                    //file.SaveAs(fileSavePath);
                    dto.PictureName = uniqueName;
                }
            }
            var pid = PMS.BAL.ProductBO.Save(dto);
            var data = new
            {
                success = true,
                ProductID = pid,
                PictureName = dto.PictureName
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveComment(CommentDTO dto)
        {
            if (!string.IsNullOrEmpty(dto.CommentText))
            {
                dto.CommentOn = DateTime.Now;
                dto.UserID = SessionManager.User.UserID;

                PMS.BAL.CommentBO.Save(dto);
                var data = new
                {
                    success = true,
                    UserName = SessionManager.User.Name,
                    CommentOn = dto.CommentOn,
                    PictureName = SessionManager.User.PictureName
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            var data1 = new
            {
                success = false,
            };
            return Json(data1, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AutoSuggestion(string val)
        {
            if (string.IsNullOrEmpty(val) || string.IsNullOrWhiteSpace(val))
                return Json("", JsonRequestBehavior.AllowGet);
            var data = PMS.BAL.ProductBO.GetMatchingItems(val);
            return Json(data, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public JsonResult GetProductByName(ProductSearchDTO dt)
        {
            var products = ProductSearchBO.GetProductByName(dt);

            var d = new
            {
                data = products
            };
            return Json(d, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLatestProducts()
        {
            var products = PMS.BAL.ProductBO.GetLatestProducts(true);

            var d = new
            {
                data = products
            };
            return Json(d, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getTrendingProducts()
        {
            var products = PMS.BAL.ProductBO.getTrendingProducts(true);

            var d = new
            {
                data = products
            };
            return Json(d, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult FAQ(string val)
        {
            if (string.IsNullOrEmpty(val) || string.IsNullOrWhiteSpace(val))
                return Json("", JsonRequestBehavior.AllowGet);
            var data = PMS.BAL.ProductBO.GetRelatedFAQ(val);
            return Json(data, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult DeleteFromWishlist(ProductDTO dto)
        {
            var user_id = SessionManager.User.UserID;
            var data = PMS.BAL.ProductBO.DeleteFromWishlist(user_id, dto.ProductID);
            return Json(data, JsonRequestBehavior.AllowGet);


        }
        public JsonResult AddToWishlist(ProductDTO dto)
        {
            var user_id = SessionManager.User.UserID;
            var data = PMS.BAL.ProductBO.AddToWishlist(user_id, dto.ProductID);
            return Json(data, JsonRequestBehavior.AllowGet);


        }



        #region Under Development

        public ActionResult Edit(int id)
        {

            var prod = ProductBO.GetProductById(id);
            var redVal = View($"New", prod);

            return redVal;

        }

        #endregion
    }
}