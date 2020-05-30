using PMS.Entities;
using System;
using System.Collections.Generic;

namespace PMS.BAL
{
    public class ProductCategoryBO
    {
        public static List<ProductCategoryDTO> GetAllCategories()
        {
            return PMS.DAL.ProductCategoryDAO.GetAllCategories();
        }
        public static int Save(String categoryName)
        {
            return PMS.DAL.ProductCategoryDAO.Save(categoryName);
        }
        public static ProductCategoryDTO GetCategory(string Categoryname)
        {
            return PMS.DAL.ProductCategoryDAO.GetCategory(Categoryname);
        }
    }
}
