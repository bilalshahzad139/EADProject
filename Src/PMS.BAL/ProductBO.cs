using PMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.BAL
{
    public static class ProductBO
    {
        public static int Save(ProductDTO dto)
        {
            return PMS.DAL.ProductDAO.Save(dto);
        }

        public static ProductDTO GetProductById(int pid)
        {
            return PMS.DAL.ProductDAO.GetProductById(pid);
        }

        public static List<ProductDTO> GetAllProducts(Boolean pLoadComments=false)
        {
            return PMS.DAL.ProductDAO.GetAllProducts(pLoadComments);
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
    }
}
