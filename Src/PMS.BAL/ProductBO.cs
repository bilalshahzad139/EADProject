using PMS.Entities;
using System;
using System.Collections.Generic;

namespace PMS.BAL
{
    public static class ProductBO
    {
        public static int Save(ProductDTO dto)
        {
            return PMS.DAL.ProductDAO.Save(dto);
        }
        public static int SaveSale(SaleDTO dto)
        {
            return PMS.DAL.ProductDAO.SaveSale(dto);
        }
        public static ProductDTO GetProductById(int pid)
        {
            return PMS.DAL.ProductDAO.GetProductById(pid);
        }

        public static int AddLikesAndGetCount(int ProductID, int UserID)
        {
            return PMS.DAL.ProductDAO.AddLikesAndGetCount(ProductID, UserID);

        }
        public static object GetSoldProductsInfo(DateTime currentDate)
        {
            return PMS.DAL.ProductDAO.GetSoldProductsInfo(currentDate);

        }
        public static int AddDisLikesAndGetCount(int ProductID, int UserID)
        {
            return PMS.DAL.ProductDAO.AddDisLikesAndGetCount(ProductID, UserID);

        }
        public static List<ProductDTO> GetAllProducts(Boolean pLoadComments = false)
        {
            return PMS.DAL.ProductDAO.GetAllProducts(pLoadComments);
        }

        public static List<ProductDTO> GetProductsByCategory(int categoryId, Boolean pLoadComments = false)
        {
            return PMS.DAL.ProductDAO.GetProductsByCategory(categoryId, pLoadComments);
        }

        public static List<ProductDTO> GetPriceRangedProducts(int from, int to, Boolean pLoadComments = false)
        {
            return PMS.DAL.ProductDAO.GetPriceRangedProducts(from, to, pLoadComments);
        }
        public static int DeleteProduct(int pid)
        {
            return PMS.DAL.ProductDAO.DeleteProduct(pid);
        }
        public static List<string> GetMatchingItems(string term)
        {
            return PMS.DAL.ProductDAO.GetMatchingItems(term);
        }
        public static int AddToWishlist(int uid, int pid)
        {
            return PMS.DAL.ProductDAO.AddToWishlist(uid, pid);
        }
        public static int test(ProductDTO dto)
        {
            return 1234;
        }
        public static List<ProductDTO> GetLatestProducts(Boolean pLoadComments = false)
        {
            return PMS.DAL.ProductDAO.GetLatestProducts(pLoadComments);
        }
        public static List<ProductDTO> getTrendingProducts(Boolean pLoadComments = false)
        {
            return PMS.DAL.ProductDAO.getTrendingProducts(pLoadComments);
        }
        public static List<string> GetRelatedFAQ(string text)
        {
            return PMS.DAL.ProductDAO.GetRelatedFAQ(text);
        }

        public static int DeleteFromWishlist(int user_id, int product_id)
        {
            return PMS.DAL.ProductDAO.DeleteFromWishlist(user_id, product_id);
        }
        public static int AddtoWishlist(int user_id, int product_id)
        {
            return PMS.DAL.ProductDAO.AddToWishlist(user_id, product_id);
        }
    }
}
